namespace Fcg.Games.Purchase.Domain.Events;
public sealed record PagamentoRecusadoEvent(
    Guid CompraId,
    string Motivo) : EventBase
{
    public override Guid StreamId { get; init; } = CompraId;
}
