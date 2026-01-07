using Fcg.Games.Purchase.Domain.ValueObjects;

namespace Fcg.Games.Purchase.Domain.Events;
public sealed record CompraIniciadaEvent(
    Guid CompraId,
    Guid UsuarioId,
    string EmailUsuario,
    List<JogoCompradoInfo> JogoInfos,
    decimal PrecoTotal) : EventBase
{
    public override Guid StreamId { get; init; } = CompraId;
}
