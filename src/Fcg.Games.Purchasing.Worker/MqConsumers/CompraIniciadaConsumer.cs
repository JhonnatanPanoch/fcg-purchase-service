using Fcg.Games.Purchase.Application.ClientsContracts.User;
using Fcg.Games.Purchase.Domain.Events;
using Fcg.Games.Purchase.Domain.Interfaces;
using MassTransit;

namespace Fcg.Games.Purchasing.Worker.MqConsumers;
public class CompraIniciadaConsumer : IConsumer<CompraIniciadaEvent>
{
    private readonly ILogger<CompraIniciadaConsumer> _logger;
    private readonly IEventStoreRepository _eventStore;
    private readonly IBus _bus;

    public CompraIniciadaConsumer(
        ILogger<CompraIniciadaConsumer> logger,
        IEventStoreRepository eventStore,
        IBus bus)
    {
        _logger = logger;
        _eventStore = eventStore;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<CompraIniciadaEvent> context)
    {
        var evento = context.Message;
        _logger.LogInformation("Evento CompraIniciada recebido! CompraId: {CompraId}", evento.CompraId);

        var processandoPagamentoEvent = new ProcessandoPagamentoEvent(evento.CompraId);
        await _eventStore.SalvarEventoAsync(processandoPagamentoEvent);

        // --- LÓGICA DE PAGAMENTO ---
        // Para simular, usa-se um valor aleatório.
        bool pagamentoAprovado = new Random().Next(0, 10) > 2; // 80% de chance de aprovar

        EventBase eventoDeResultado;

        if (pagamentoAprovado)
        {
            _logger.LogInformation("Pagamento para CompraId {CompraId} foi APROVADO.", evento.CompraId);
            eventoDeResultado = new PagamentoAprovadoEvent(evento.CompraId, "jhonnatan.jp@gmail.com");//evento.EmailUsuario);
        }
        else
        {
            _logger.LogError("Pagamento para CompraId {CompraId} foi RECUSADO.", evento.CompraId);
            eventoDeResultado = new PagamentoRecusadoEvent(evento.CompraId, "Saldo insuficiente");
        }

        await _eventStore.SalvarEventoAsync(eventoDeResultado);

        await _bus.Publish((object)eventoDeResultado);

        _logger.LogInformation("Compra {CompraId} processada.", evento.CompraId);
    }
}
