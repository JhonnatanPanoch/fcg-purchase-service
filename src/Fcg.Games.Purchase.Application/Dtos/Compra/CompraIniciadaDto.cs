using Fcg.Games.Purchase.Domain.Enums;

namespace Fcg.Games.Purchase.Application.Dtos.Compra;
public class CompraIniciadaDto
{
    public Guid IdComprovante { get; set; }
    public string Status { get; set; }
    public decimal PrecoPago { get; set; }

    public CompraIniciadaDto(
        Guid idComprovante,
        decimal precoPago)
    {
        IdComprovante = idComprovante;
        PrecoPago = precoPago;
        Status = EStatusCompra.Processando.ToString();
    }
}
