using SearchJobs.Api.Interfaces;

namespace SearchJobs.Api;

public class SqsPollingBackgroundService(IMessagesHandler handler, IConfiguration configuration, ILogger<SqsPollingBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        var queueUrl = configuration.GetSection("QueueUrl:PrimaryQueue").Get<string>();

        if (string.IsNullOrEmpty(queueUrl))
            throw new ArgumentException("QueueUrl:PrimaryQueue is empty");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await handler.GetMessageAsync(queueUrl, 10, stoppingToken);
            }
            catch (NullReferenceException exception)
            {
                logger.LogError("Empty queue {QueueURL}. \nException details: {Details}", queueUrl, exception.Message);
            }
            catch (Exception exception)
            {
                logger.LogError("Unexpected error when polling {QueueURL}", queueUrl);
            }
        }

        // return Task.CompletedTask;
    }
}
