using SearchJobs.Api.Models;

namespace SearchJobs.Api.Interfaces;

public interface IDispatchServiceMessagesHandler
{
    Task<(DispatchWriterEvent? Event, string? ReceiptHandle)> GetMessageAsync(string queueUrl, int waitTime, CancellationToken stoppingToken);

    Task DeleteMessageAsync(string queueUrl, string receiptHandle);
}
