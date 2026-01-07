namespace Fcg.Games.Purchase.Application.ClientsContracts.Jogo;
public class CalculoPrecoResponseDto
{
    public CalculoPrecoResponseDto()
    {
        Jogos = [];
    }

    public List<JogoPrecoDto> Jogos { get; set; }
}
