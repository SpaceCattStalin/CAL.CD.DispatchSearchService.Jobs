using SearchJobs.Api.Interfaces;
using SearchJobs.Api.Models;

namespace SearchJobs.Api.JobProcessors.DispatchQueueProcessor;

public class DispatchJobProcessor(IDispatchSearchServiceClient searchClient, IMessagesHandler messagesHandler)
                                                                                    : IDispatchJobProcessor
{
    public async Task ProcessIndexAsync(DispatchModel dispatchModel, string queueUrl, string receiptHandle, CancellationToken ct)
    {
        await searchClient.IndexAsync(dispatchModel, ct);
        await messagesHandler.DeleteMessageAsync(queueUrl, receiptHandle);
    }

    public async Task ProcessDeleteAsync(Guid dispatchId, string queueUrl, string receiptHandle, CancellationToken ct)
    {
        await searchClient.DeleteAsync(dispatchId, ct);
        await messagesHandler.DeleteMessageAsync(queueUrl, receiptHandle);
    }
}
