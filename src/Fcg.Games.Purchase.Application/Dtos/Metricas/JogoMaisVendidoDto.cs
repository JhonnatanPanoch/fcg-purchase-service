namespace Fcg.Games.Purchase.Application.Dtos.Metricas;
public class JogoMaisVendidoDto
{
    public JogoMaisVendidoDto(Guid jogoId, int quantidadeVendas)
    {
        JogoId = jogoId;
        QuantidadeVendas = quantidadeVendas;
    }

    public Guid JogoId { get; set; }
    public int QuantidadeVendas { get; set; }
}
