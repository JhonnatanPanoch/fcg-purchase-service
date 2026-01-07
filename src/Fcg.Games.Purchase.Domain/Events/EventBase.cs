namespace Fcg.Games.Purchase.Domain.Events;
public abstract record EventBase
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public abstract Guid StreamId { get; init; }
}
