using Fcg.Games.Purchase.Domain.Enums;

namespace Fcg.Games.Purchase.Domain.Entities;
public class TransacaoJogosEntity : EntityBase
{
    public Guid CompraId { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid JogoId { get; set; }
    public DateTime DataAquisicao { get; set; }
    public decimal PrecoPago { get; set; }
    public EStatusCompra Status { get; set; }
}