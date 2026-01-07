using Fcg.Games.Purchase.Application.ClientsContracts.Jogo;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Fcg.Games.Purchase.Infra.Clients.Jogo;

public class GamesServiceClient(
    IHttpContextAccessor httpContextAccessor,
    IHttpClientFactory httpClientFactory,
    ILogger<BaseHttpClient> logger) : BaseHttpClient(
        httpContextAccessor,
        httpClientFactory, 
        logger, 
        "GamesService"), IGamesServiceClient
{
    public async Task<CalculoPrecoResponseDto> ConsultarPrecosAsync(List<Guid> jogoIds)
    {
        if (jogoIds is null || !jogoIds.Any())
            return new CalculoPrecoResponseDto();

        var requestUri = $"api/v1/jogos/consultar-precos?ids={string.Join("&ids=", jogoIds)}";

        return await GetAsync<CalculoPrecoResponseDto>(requestUri);
    }
}