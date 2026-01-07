using Fcg.Games.Purchase.Domain.Enums;

namespace Fcg.Games.Purchase.Application.Dtos.Transacao;
public class TransacaoJogosDto
{
    public Guid JogoId { get; set; }
    public DateTime DataAquisicao { get; set; }
    public decimal PrecoPago { get; set; }
}