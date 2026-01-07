namespace Fcg.Games.Purchase.Application.ClientsContracts.User;
public class JogosContaDto
{
    public Guid IdComprovante { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public DateTime DataAquisicao { get; set; }
    public decimal ValorPago { get; set; }
}
