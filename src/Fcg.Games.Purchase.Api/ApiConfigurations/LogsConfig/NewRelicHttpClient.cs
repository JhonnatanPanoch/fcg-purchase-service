using Serilog.Sinks.Http;

namespace Fcg.Games.Purchase.Api.ApiConfigurations.LogsConfig;

public class NewRelicHttpClient : IHttpClient
{
    private readonly HttpClient _httpClient;

    public NewRelicHttpClient()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Api-Key", "0dbf2540aad7a0756c29a36d5e151db7FFFFNRAL");
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