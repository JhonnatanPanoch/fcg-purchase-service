using Microsoft.Extensions.Options;
using Serilog.Sinks.Http;

namespace Fcg.Games.Purchasing.Worker.Configurations;

public class NewRelicHttpClient : IHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly NewRelicSettings _settings;

    public NewRelicHttpClient(IOptions<NewRelicSettings> settings)
    {
        _httpClient = new HttpClient();
        _settings = settings.Value;
        _httpClient.DefaultRequestHeaders.Add("Api-Key", _settings.SecretKey);
    }

    public void Configure(IConfiguration configuration)
    {
    }

    public Task<HttpResponseMessage> PostAsync(string requestUri, Stream contentStream)
    {
        var content = new StreamContent(contentStream);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        return _httpClient.PostAsync(requestUri, content);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}

