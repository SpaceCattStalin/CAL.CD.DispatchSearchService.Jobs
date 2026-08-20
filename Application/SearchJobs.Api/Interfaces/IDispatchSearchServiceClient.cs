using SearchJobs.Api.Models;

namespace SearchJobs.Api;

public interface IDispatchSearchServiceClient
{
    Task<string> IndexAsync(DispatchModel dispatchModel, CancellationToken ct);
    Task DeleteAsync(Guid dispatchId, CancellationToken ct);
}
