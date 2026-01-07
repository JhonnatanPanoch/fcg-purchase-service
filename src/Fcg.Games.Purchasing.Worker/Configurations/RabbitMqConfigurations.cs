using Fcg.Games.Purchasing.Worker.MqConsumers;
using MassTransit;

namespace Fcg.Games.Purchasing.Worker.Configurations;
public static class RabbitMqConfigurations
{

    public static void ConfigureMqConsumers(this IRabbitMqBusFactoryConfigurator cfg, IBusRegistrationContext context)
    {
        cfg.ReceiveEndpoint("fila-compras-iniciadas", endpointConfigurator =>
        {
            endpointConfigurator.ConfigureConsumer<CompraIniciadaConsumer>(context);
        });

        cfg.ReceiveEndpoint("fila-atualiza-status-compra-worker", endpointConfigurator =>
        {
            endpointConfigurator.ConfigureConsumer<AtualizaStatusCompraConsumer>(context);
        });
    }
}
