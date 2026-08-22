using System.Net;
using System.Net.Http.Json;
using SearchJobs.Api.Models;

namespace SearchJobs.Api;

public class DispatchSearchServiceClient(HttpClient httpClient) : IDispatchSearchServiceClient
{
    public async Task<string> IndexAsync(DispatchModel dispatchModel, CancellationToken ct)
    {
        var response = await httpClient.PostAsJsonAsync("/api/dispatch", dispatchModel, ct);

        response.EnsureSuccessStatusCode(); // non-2xx throws -> Hangfire retries the job

        var body = await response.Content.ReadFromJsonAsync<IndexResult>(ct);
        return body?.Id
            ?? throw new InvalidOperationException("SearchService returned an empty response body for POST api/dispatch.");
    }

    public async Task DeleteAsync(Guid dispatchId, CancellationToken ct)
    {
        var response = await httpClient.DeleteAsync($"api/dispatch/{dispatchId}", ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return; // already gone — delete is idempotent, not an error

        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateAsync(DispatchModel dispatchModel, CancellationToken ct)
    {
        var response = await httpClient.PutAsJsonAsync("/api/dispatch", dispatchModel, ct);

        response.EnsureSuccessStatusCode();
    }


    internal sealed record IndexResult(string Id);
}
