using System.Net.Http.Json;
using SearchJobs.Api.Models;

namespace SearchJobs.Api;

public class DispatchServiceClient(HttpClient httpClient) : IDispatchServiceClient
{
    public async Task<PageResponseWithCursor<DispatchWriterDto>> GetAsync(string cursor, int limit = 500)
    {
        var response = await httpClient.GetAsync($"dispatch?cursor={Uri.EscapeDataString(cursor)}&limit={limit}");

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<PageResponseWithCursor<DispatchWriterDto>>();
        
        return body
            ?? throw new InvalidOperationException("DispatchService returned an empty response body for GET dispatch.");
    }
}
