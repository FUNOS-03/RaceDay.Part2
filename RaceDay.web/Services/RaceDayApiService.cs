
using System.Net.Http.Json;

namespace RaceDay.web.Services;

public class RaceDayApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RaceDayApiService(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient("RaceDayAPI");

        var token = _httpContextAccessor.HttpContext?
            .Session.GetString("JWToken");

        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", token);
        }

        return client;
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        var client = CreateClient();
        return await client.GetFromJsonAsync<T>(endpoint);
    }

    public async Task<HttpResponseMessage> PostAsync<T>(
        string endpoint, T data)
    {
        var client = CreateClient();
        return await client.PostAsJsonAsync(endpoint, data);
    }

    public async Task<HttpResponseMessage> PutAsync<T>(
        string endpoint, T data)
    {
        var client = CreateClient();
        return await client.PutAsJsonAsync(endpoint, data);
    }

    public async Task<HttpResponseMessage> DeleteAsync(
        string endpoint)
    {
        var client = CreateClient();
        return await client.DeleteAsync(endpoint);
    }
}
