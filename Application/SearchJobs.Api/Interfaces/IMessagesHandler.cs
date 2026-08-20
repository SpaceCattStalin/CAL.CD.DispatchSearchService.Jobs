namespace SearchJobs.Api.Interfaces;

public interface IMessagesHandler
{
    Task GetMessageAsync(string queueUrl, int waitTime, CancellationToken stoppingToken);
}
