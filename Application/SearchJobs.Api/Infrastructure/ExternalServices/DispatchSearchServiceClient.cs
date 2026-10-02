using System.Net;
using System.Net.Http.Json;
using Hangfire;
using SearchJobs.Api.Models;

namespace SearchJobs.Api;

public class DispatchSearchServiceClient(HttpClient httpClient, ILogger<DispatchSearchServiceClient> _logger) : IDispatchSearchServiceClient
{
    public async Task<string> IndexAsync(DispatchModel dispatchModel, CancellationToken ct)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("api/dispatch", dispatchModel, ct);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadFromJsonAsync<IndexResult>(ct);
            return body?.Id;
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException("SearchService returned an empty response body for POST api/dispatch. {0}", ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fail to update dispatch {0}", ex);
            return String.Empty;
        }
    }

    public async Task DeleteAsync(Guid dispatchId, CancellationToken ct)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"api/dispatch/{dispatchId}", ct);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return;

            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fail to delete dispatch {0}", ex);
        }
    }
    public async Task UpdateAsync(DispatchModel dispatchModel, CancellationToken ct)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync("api/dispatch", dispatchModel, ct);

            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fail to update dispatch {0}", ex);
        }
    }

    public async Task BatchUpsertAsync(List<DispatchModel> dispatchModels)
    {
        _logger.LogCritical("Total count {Count}", dispatchModels.Count);

        var response = await httpClient.PutAsJsonAsync("api/dispatch/batch-update", new { Documents = dispatchModels });
        
        response.EnsureSuccessStatusCode();
    }

    internal sealed record IndexResult(string Id);
}
