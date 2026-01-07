namespace Fcg.Games.Purchase.Domain.ValueObjects;
public sealed record JogoCompradoInfo(
    Guid JogoId,
    decimal PrecoUnitario
);