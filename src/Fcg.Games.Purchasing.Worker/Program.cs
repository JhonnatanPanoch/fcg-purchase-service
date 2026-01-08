using Fcg.Games.Purchase.Domain.Interfaces;
using Fcg.Games.Purchase.Infra.Contexts;
using Fcg.Games.Purchase.Infra.Repositories;
using Fcg.Games.Purchasing.Worker;
using Fcg.Games.Purchasing.Worker.Configurations;
using Fcg.Games.Purchasing.Worker.MqConsumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Sinks.Http.BatchFormatters;

var builder = Host.CreateApplicationBuilder(args);

#region NewRelic
builder.Services.Configure<NewRelicSettings>(builder.Configuration.GetSection("NewRelicSettings"));

var newRelicSettings = builder.Configuration.GetSection("NewRelicSettings").Get<NewRelicSettings>()
    ?? throw new InvalidOperationException("NewRelicSettings não foi configurado corretamente.");
var newRelicOptions = Options.Create(newRelicSettings);

Log.Logger = new LoggerConfiguration()
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

builder.Services.AddSerilog();
#endregion

builder.Services.AddHostedService<Worker>();

#region database

builder.Services.AddDbContext<AppDbContext>(options => options
    .UseLazyLoadingProxies()
    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

#endregion

#region DIs

/// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IEventStoreRepository, EventStoreRepository>();

#endregion

#region Masstransit
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddConsumer<CompraIniciadaConsumer>();
    busConfigurator.AddConsumer<AtualizaStatusCompraConsumer>();

    busConfigurator.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["MessageBroker:Host"], 5671, "/", hostConfigurator =>
        {
            hostConfigurator.Username(builder.Configuration["MessageBroker:Username"]);
            hostConfigurator.Password(builder.Configuration["MessageBroker:Password"]);
            hostConfigurator.UseSsl(s => { });
        });

        cfg.ConfigureMqConsumers(context);
    });
});
#endregion

var host = builder.Build();

host.Run();