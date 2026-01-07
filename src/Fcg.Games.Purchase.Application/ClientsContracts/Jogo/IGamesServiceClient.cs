namespace Fcg.Games.Purchase.Application.ClientsContracts.Jogo;
public interface IGamesServiceClient
{
    /// <summary>
    /// Consulta a API de Jogos para obter os preços de uma lista de IDs de jogos.
    /// </summary>
    /// <param name="jogoIds">A lista de IDs dos jogos a serem consultados.</param>
    /// <returns>Um DTO com a lista de jogos e seus preços, ou nulo se a chamada falhar.</returns>
    Task<CalculoPrecoResponseDto> ConsultarPrecosAsync(List<Guid> jogoIds);
}
