using Fcg.Games.Purchase.Domain.Interfaces;
using Fcg.Games.Purchase.Infra.Contexts;
using Fcg.Games.Purchase.Infra.Repositories;
using Fcg.Games.Purchasing.Worker;
using Fcg.Games.Purchasing.Worker.Configurations;
using Fcg.Games.Purchasing.Worker.MqConsumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

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