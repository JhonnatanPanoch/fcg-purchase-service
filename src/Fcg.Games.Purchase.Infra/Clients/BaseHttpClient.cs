using Fcg.Games.Purchase.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Fcg.Games.Purchase.Infra.Clients;
public abstract class BaseHttpClient
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<BaseHttpClient> _logger;
    private readonly string _clientName;

    protected BaseHttpClient(
        IHttpContextAccessor httpContextAccessor,
        IHttpClientFactory httpClientFactory,
        ILogger<BaseHttpClient> logger,
        string clientName)
    {
        _httpContextAccessor = httpContextAccessor;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _clientName = clientName;
    }

    protected async Task<T?> GetAsync<T>(string requestUri, CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient(_clientName);
        
        SetJwtToken(client);

        try
        {
            _logger.LogInformation("Realizando GET para o client {ClientName} em: {RequestUri}", _clientName, requestUri);
            var response = await client.GetAsync(requestUri, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Falha na chamada para {ClientName}. Status Code: {StatusCode}", _clientName, response.StatusCode);
                throw new ApplicationErrorException($"Falha na comunicação com o serviço {_clientName}.");
            }

            var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return await JsonSerializer.DeserializeAsync<T>(responseStream, options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao chamar o serviço {ClientName}.", _clientName);
            throw new ApplicationErrorException($"Erro inesperado na comunicação com o serviço {_clientName}.", ex);
        }
    }

    private void SetJwtToken(HttpClient client)
    {
        var context = _httpContextAccessor?.HttpContext;
        if (context == null)
            return;

        if (context.Request.Headers.TryGetValue("Authorization", out StringValues headerValues) && !StringValues.IsNullOrEmpty(headerValues))
        {
            var header = headerValues.ToString(); 
                                                  
            if (AuthenticationHeaderValue.TryParse(header, out var parsed))
            {
                client.DefaultRequestHeaders.Authorization = parsed;
            }
        }
    }
}
