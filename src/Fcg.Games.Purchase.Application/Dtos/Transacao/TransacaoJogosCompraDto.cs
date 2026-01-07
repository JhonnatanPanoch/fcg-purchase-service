namespace Fcg.Games.Purchase.Application.Dtos.Transacao;

public class TransacaoJogosCompraDto
{
    public Guid CompraId { get; set; }
    public IEnumerable<TransacaoJogosDto> Jogos { get; set; }
}
