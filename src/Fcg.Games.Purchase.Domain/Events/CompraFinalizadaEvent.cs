namespace Fcg.Games.Purchase.Domain.Events;

public sealed record CompraFinalizadaEvent(
    Guid CompraId) : EventBase
{
    public override Guid StreamId { get; init; } = CompraId;
}