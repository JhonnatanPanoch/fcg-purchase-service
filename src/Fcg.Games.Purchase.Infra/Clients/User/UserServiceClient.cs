using Fcg.Games.Purchase.Application.ClientsContracts.User;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Fcg.Games.Purchase.Infra.Clients.User;

public class UserServiceClient(
    IHttpContextAccessor httpContextAccessor,
    IHttpClientFactory httpClientFactory,
    ILogger<BaseHttpClient> logger) : BaseHttpClient(
        httpContextAccessor,
        httpClientFactory,
        logger,
        "UserService"), IUserServiceClient
{
    public async Task<UsuarioDto> ConsultarPorIdAsync(Guid id)
    {
        return await GetAsync<UsuarioDto>($"api/v1/usuario/{id}");
    }

    public async Task<ContaDto> ConsultarDadosContaAsync()
    {
        return await GetAsync<ContaDto>($"api/v1/conta");
    }
}