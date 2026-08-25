using System.Net.Http.Json;
using SearchJobs.Api.Models;

namespace SearchJobs.Api;

public class DispatchServiceClient(HttpClient httpClient) : IDispatchServiceClient
{
    public async Task<PageResponseWithCursor<DispatchWriterDto>> GetAsync(string cursor, int limit = 500)
    {
        var response = await httpClient.GetAsync($"api/dispatch?cursor={Uri.EscapeDataString(cursor)}&limit={limit}");

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<PageResponseWithCursor<DispatchWriterDto>>();

        return body
            ?? throw new InvalidOperationException("DispatchService returned an empty response body for GET dispatch.");
    }

    public async Task<PageResponseWithCursor<DispatchWriterDto>> GetAsync(string cursor, string auth, int limit = 500)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/dispatch?cursor={Uri.EscapeDataString(cursor)}&limit={limit}");
        request.Headers.Add("Authorization", auth);

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<PageResponseWithCursor<DispatchWriterDto>>();

        return body
            ?? throw new InvalidOperationException("DispatchService returned an empty response body for GET dispatch.");
    }

}
