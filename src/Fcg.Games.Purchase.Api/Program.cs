using Asp.Versioning;
using Fcg.Games.Purchase.Api.ApiConfigurations;
using Fcg.Games.Purchase.Api.ApiConfigurations.LogsConfig;
using Fcg.Games.Purchase.Api.Filters;
using Fcg.Games.Purchase.Api.Middlewares;
using Fcg.Games.Purchase.Application.ApiSettings;
using Fcg.Games.Purchase.Application.AppServices;
using Fcg.Games.Purchase.Application.ClientsContracts.Jogo;
using Fcg.Games.Purchase.Application.ClientsContracts.User;
using Fcg.Games.Purchase.Application.Interfaces;
using Fcg.Games.Purchase.Domain.Interfaces;
using Fcg.Games.Purchase.Domain.Interfaces.Services;
using Fcg.Games.Purchase.Domain.Services;
using Fcg.Games.Purchase.Infra.Clients.Jogo;
using Fcg.Games.Purchase.Infra.Clients.User;
using Fcg.Games.Purchase.Infra.Contexts;
using Fcg.Games.Purchase.Infra.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.Http.BatchFormatters;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region database

builder.Services.AddDbContext<AppDbContext>(options => options
    .UseLazyLoadingProxies()
    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

#endregion

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateGuidQueryParamsFilter>();
});

builder.Services.AddHealthChecks();
builder.Services.AddHttpContextAccessor();

#region DIs

/// Applications
builder.Services.AddAbstractValidations();
builder.Services.AddScoped<ICompraAppService, CompraAppService>();
builder.Services.AddScoped<IUsuarioAutenticadoAppService, UsuarioAutenticadoAppService>();
builder.Services.AddScoped<ITransacaoAppService, TransacaoAppService>();
builder.Services.AddScoped<IMetricaAppService, MetricaAppService>();

/// Domains
builder.Services.AddScoped<ICompraService, CompraService>();

/// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IEventStoreRepository, EventStoreRepository>();

/// Clients
builder.Services.AddScoped<IGamesServiceClient, GamesServiceClient>();
builder.Services.AddScoped<IUserServiceClient, UserServiceClient>();

#endregion

#region Swagger

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Serviço de Pagamentos", Description = "Este serviço gerencia todo o fluxo de compras, registrando as transações e efetuando o pagamento.", Version = "v1" });
    c.EnableAnnotations();
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Insira o token JWT",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            securityScheme,
            new[] { "Bearer" }
        }
    };

    c.AddSecurityRequirement(securityRequirement);
});

#endregion

#region NewRelic
builder.Services.Configure<NewRelicSettings>(builder.Configuration.GetSection("NewRelicSettings"));

var newRelicSettings = builder.Configuration.GetSection("NewRelicSettings").Get<NewRelicSettings>()
    ?? throw new InvalidOperationException("NewRelicSettings não foi configurado corretamente.");
var newRelicOptions = Options.Create(newRelicSettings);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown")
    .WriteTo.Console()
    .WriteTo.DurableHttpUsingFileSizeRolledBuffers(
        requestUri: "https://log-api.newrelic.com/log/v1",
        textFormatter: new NewRelicFormatter(),
        batchFormatter: new ArrayBatchFormatter(),
        httpClient: new NewRelicHttpClient(newRelicOptions))
    .CreateLogger();

builder.Host.UseSerilog();
#endregion

#region Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
})
.AddMvc()
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});
#endregion

#region Jwt
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings?.Issuer,
        ValidAudience = jwtSettings?.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.SecretKey)),
        RoleClaimType = ClaimTypes.Role,
    };
});

builder.Services.AddAuthorization();
#endregion

#region MassTransit
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["MessageBroker:Host"], 5671, "/", hostConfigurator =>
        {
            hostConfigurator.Username(builder.Configuration["MessageBroker:Username"]);
            hostConfigurator.Password(builder.Configuration["MessageBroker:Password"]);
            hostConfigurator.UseSsl(s => { });
        });
    });
});
#endregion

#region HttpClientFactories
builder.Services.AddHttpClient("GamesService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:GamesApiUrl"]!);
});
builder.Services.AddHttpClient("UserService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:UsuarioApiUrl"]!);
});
#endregion

var app = builder.Build();

#region Migrations

await app.ApplyMigrationsWithSeedsAsync();

#endregion

#region Middlewares

app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

#endregion

app.Run();

public partial class Program { }