using SearchJobs.Api.Interfaces;
using SearchJobs.Api.Models;

namespace SearchJobs.Api;

public class SqsPollingBackgroundService(IMessagesHandler handler, IJobEnqueuer<IDispatchSearchServiceClient> jobEnqueuer, IConfiguration configuration, ILogger<SqsPollingBackgroundService> logger) : BackgroundService
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
                var job = await handler.GetMessageAsync(queueUrl, 10, stoppingToken);

                jobEnqueuer.Enqueue(client => client.IndexAsync(job.ToDispatchModel(), stoppingToken));
            }
            catch (NullReferenceException exception)
            {
                logger.LogError("Empty queue {QueueURL}. \nException details: {Details}", queueUrl, exception.Message);
            }
            catch (ArgumentNullException exception)
            {
                logger.LogError("Null argument {QueueURL}. \nException details: {Details}", queueUrl, exception.Message);
            }
            catch (Exception exception)
            {
                logger.LogError("Unexpected error when polling {QueueURL}", queueUrl);
            }
        }

        // return Task.CompletedTask;
    }
}
