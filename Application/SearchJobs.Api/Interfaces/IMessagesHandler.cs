using SearchJobs.Api.Models;

namespace SearchJobs.Api.Interfaces;

public interface IMessagesHandler
{
    Task<DispatchWriterEvent> GetMessageAsync(string queueUrl, int waitTime, CancellationToken stoppingToken);
}
