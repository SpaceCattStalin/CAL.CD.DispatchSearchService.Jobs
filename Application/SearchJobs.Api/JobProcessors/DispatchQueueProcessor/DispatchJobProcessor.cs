using SearchJobs.Api.Interfaces;
using SearchJobs.Api.Models;

namespace SearchJobs.Api.JobProcessors.DispatchQueueProcessor;

public class DispatchJobProcessor(IDispatchSearchServiceClient searchClient, IDispatchServiceMessagesHandler messagesHandler)
                                                                                    : IDispatchJobProcessor
{
    public async Task ProcessIndexAsync(DispatchModel dispatchModel, string queueUrl, string receiptHandle, CancellationToken ct)
    {
        // Should use CreateAsync as it is more explicit than IndexAsync (IndexAsync is used for upsert operation)
        await searchClient.IndexAsync(dispatchModel, ct);
        await messagesHandler.DeleteMessageAsync(queueUrl, receiptHandle);
    }

    public async Task ProcessDeleteAsync(Guid dispatchId, string queueUrl, string receiptHandle, CancellationToken ct)
    {
        await searchClient.DeleteAsync(dispatchId, ct);
        await messagesHandler.DeleteMessageAsync(queueUrl, receiptHandle);
    }

    public async Task ProcessUpdateAsync(DispatchModel dispatchModel, string queueUrl, string receiptHandle, CancellationToken ct)
    {
        await searchClient.UpdateAsync(dispatchModel, ct);
        await messagesHandler.DeleteMessageAsync(queueUrl, receiptHandle);
    }

}
