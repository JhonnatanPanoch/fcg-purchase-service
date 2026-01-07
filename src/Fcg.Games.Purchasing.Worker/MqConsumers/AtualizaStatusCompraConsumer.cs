using Fcg.Games.Purchase.Application.ClientsContracts.User;
using Fcg.Games.Purchase.Domain.Entities;
using Fcg.Games.Purchase.Domain.Enums;
using Fcg.Games.Purchase.Domain.Events;
using Fcg.Games.Purchase.Domain.Interfaces;
using MassTransit;

namespace Fcg.Games.Purchasing.Worker.MqConsumers;
public class AtualizaStatusCompraConsumer :
    IConsumer<PagamentoAprovadoEvent>,
    IConsumer<PagamentoRecusadoEvent>
{
    private readonly IRepository<TransacaoJogosEntity> _aquisicaoRepository;
    public readonly IEventStoreRepository _eventStoreRepository;
    private readonly ILogger<AtualizaStatusCompraConsumer> _logger;

    public AtualizaStatusCompraConsumer(
        IRepository<TransacaoJogosEntity> aquisicaoRepository,
        ILogger<AtualizaStatusCompraConsumer> logger,
        IEventStoreRepository eventStoreRepository)
    {
        _aquisicaoRepository = aquisicaoRepository;
        _logger = logger;
        _eventStoreRepository = eventStoreRepository;
    }

    // Executado quando o pagamento é APROVADO
    public async Task Consume(ConsumeContext<PagamentoAprovadoEvent> context)
    {
        var eventoAprovado = context.Message;
        _logger.LogInformation("Evento PagamentoAprovadoEvent recebido para a CompraId: {CompraId}", eventoAprovado.StreamId);

        try
        {
            var eventoIniciado = await _eventStoreRepository.ObterEventoAsync<CompraIniciadaEvent>(eventoAprovado.StreamId);
            if (eventoIniciado is null)
            {
                _logger.LogError("Evento CompraIniciadaEvent não encontrado para o StreamId: {StreamId}", eventoAprovado.StreamId);
                return;
            }

            foreach (var jogoId in eventoIniciado.JogoInfos)
            {
                var novaAquisicao = new TransacaoJogosEntity
                {
                    Id = Guid.NewGuid(),
                    CompraId = eventoIniciado.CompraId,
                    UsuarioId = eventoIniciado.UsuarioId,
                    JogoId = jogoId.JogoId,
                    DataAquisicao = DateTime.UtcNow,
                    PrecoPago = jogoId.PrecoUnitario,
                    Status = EStatusCompra.Finalizado
                };
                await _aquisicaoRepository.AdicionarAsync(novaAquisicao);
            }
            _logger.LogInformation("Read Model (Aquisicao) criado com sucesso para a CompraId: {CompraId}", eventoAprovado.StreamId);

            await _eventStoreRepository.SalvarEventoAsync(new CompraFinalizadaEvent(eventoIniciado.CompraId));
            _logger.LogInformation("Compra {CompraId} finalizada.", eventoAprovado.StreamId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar PagamentoAprovadoEvent para a CompraId: {CompraId}", eventoAprovado.StreamId);
            throw;
        }
    }

    // Executado quando o pagamento é RECUSADO
    public async Task Consume(ConsumeContext<PagamentoRecusadoEvent> context)
    {
        var eventoRecusado = context.Message;
        _logger.LogWarning("Evento PagamentoRecusadoEvent recebido para a CompraId: {CompraId}. Motivo: {Motivo}", eventoRecusado.StreamId, eventoRecusado.Motivo);

        var eventoIniciado = await _eventStoreRepository.ObterEventoAsync<CompraIniciadaEvent>(eventoRecusado.StreamId);
        if (eventoIniciado is null) return;

        foreach (var jogo in eventoIniciado.JogoInfos)
        {
            var aquisicaoFalha = new TransacaoJogosEntity
            {
                Id = Guid.NewGuid(),
                CompraId = eventoIniciado.CompraId,
                UsuarioId = eventoIniciado.UsuarioId,
                JogoId = jogo.JogoId,
                DataAquisicao = DateTime.UtcNow,
                PrecoPago = jogo.PrecoUnitario,
                Status = EStatusCompra.ErroProcessamento
            };

            await _aquisicaoRepository.AdicionarAsync(aquisicaoFalha);
        }
    }
}
