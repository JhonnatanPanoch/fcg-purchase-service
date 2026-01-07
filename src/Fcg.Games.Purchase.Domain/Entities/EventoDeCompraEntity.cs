namespace Fcg.Games.Purchase.Domain.Entities;
public class EventoDeCompraEntity : EntityBase
{
    public Guid StreamId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string EventData { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
