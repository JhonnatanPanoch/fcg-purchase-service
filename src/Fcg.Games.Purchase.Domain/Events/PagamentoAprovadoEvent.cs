namespace Fcg.Games.Purchase.Domain.Events;

public sealed record PagamentoAprovadoEvent(
    Guid CompraId,
    string EmailUsuario) : EventBase
{
    public override Guid StreamId { get; init; } = CompraId;
}