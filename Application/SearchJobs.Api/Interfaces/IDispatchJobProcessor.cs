using SearchJobs.Api.Models;

namespace SearchJobs.Api;

public interface IDispatchJobProcessor
{
    Task ProcessIndexAsync(DispatchModel dispatchModel, string queueUrl, string receiptHandle, CancellationToken ct);
    Task ProcessDeleteAsync(Guid dispatchId, string queueUrl, string receiptHandle, CancellationToken ct);
}
