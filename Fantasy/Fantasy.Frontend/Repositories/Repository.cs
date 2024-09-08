
using System.Text;
using System.Text.Json;

namespace Fantasy.Frontend.Repositories;

public class Repository : IRepository
{
    private readonly HttpClient _httpClient;

    public Repository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private JsonSerializerOptions _jsonDefaultOptions =>
        new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

    public async Task<HttpResponseWrapper<object>> DeleteAsync(string url)
    {
        var responseHttp = _httpClient.DeleteAsync(url);
        return new HttpResponseWrapper<object>(null, !responseHttp.Result.IsSuccessStatusCode, responseHttp.Result);
    }

    public async Task<HttpResponseWrapper<T>> GetAsync<T>(string url)
    {
        var responseHttp = await _httpClient.GetAsync(url);
        if (responseHttp.IsSuccessStatusCode)
        {
            var response = await UnserializeAnswer<T>(responseHttp);
            return new HttpResponseWrapper<T>(response, false, responseHttp);
        }

        return new HttpResponseWrapper<T>(default, true, responseHttp);
    }

    public async Task<HttpResponseWrapper<object>> PostAsync<T>(string url, T model)
    {
        var messageJSON = JsonSerializer.Serialize(model);
        var messageContent = new StringContent(messageJSON, Encoding.UTF8, "application/json");
        var responseHttp = await _httpClient.PostAsync(url, messageContent);
        return new HttpResponseWrapper<object>(null, !responseHttp.IsSuccessStatusCode, responseHttp);
    }

    public async Task<HttpResponseWrapper<TActionResponse>> PostAsync<T, TActionResponse>(string url, T model)
    {
        var messageJSON = JsonSerializer.Serialize(model);
        var messageContent = new StringContent(messageJSON, Encoding.UTF8, "application/json");
        var responseHttp = await _httpClient.PostAsync(url, messageContent);
        if (responseHttp.IsSuccessStatusCode)
        {
            var response = await UnserializeAnswer<TActionResponse>(responseHttp);
            return new HttpResponseWrapper<TActionResponse>(response, false, responseHttp);
        }
        return new HttpResponseWrapper<TActionResponse>(default, !responseHttp.IsSuccessStatusCode, responseHttp);
    }

    public async Task<HttpResponseWrapper<object>> PutAsync<T>(string url, T model)
    {
        var messageJson = JsonSerializer.Serialize(model);
        var messageContent = new StringContent(messageJson, Encoding.UTF8, "application/json");
        var responseHttp = _httpClient.PutAsync(url, messageContent);
        return new HttpResponseWrapper<object>(null, !responseHttp.Result.IsSuccessStatusCode, responseHttp.Result);
    }

    public async Task<HttpResponseWrapper<TActionResponse>> PutAsync<T, TActionResponse>(string url, T model)
    {
        var messageJson = JsonSerializer.Serialize(model);
        var messageContent = new StringContent(messageJson, Encoding.UTF8, "application/json");
        var responseHttp = _httpClient.PutAsync(url, messageContent);
        if (responseHttp.Result.IsSuccessStatusCode)
        {
            var response = await UnserializeAnswer<TActionResponse>(responseHttp.Result);
            return new HttpResponseWrapper<TActionResponse>(response, false, responseHttp.Result);
        }
        return new HttpResponseWrapper<TActionResponse>(default, !responseHttp.Result.IsSuccessStatusCode, responseHttp.Result);
    }

    private async Task<T> UnserializeAnswer<T>(HttpResponseMessage responseHttp)
    {
        var response = await responseHttp.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<T>(response, _jsonDefaultOptions);
        if (result is null)
        {
            throw new JsonException("Failed to deserialize response.");
        }
        return result;
    }
}
