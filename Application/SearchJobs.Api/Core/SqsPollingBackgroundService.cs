using SearchJobs.Api.Interfaces;
using SearchJobs.Api.Models;

namespace SearchJobs.Api;

public class SqsPollingBackgroundService(IMessagesHandler handler, IJobEnqueuer<IDispatchJobProcessor> jobEnqueuer, IConfiguration configuration, ILogger<SqsPollingBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        var queueUrl = configuration.GetSection("QueueUrl:PrimaryQueue").Get<string>();

        if (string.IsNullOrEmpty(queueUrl))
            throw new ArgumentException("QueueUrl:PrimaryQueue is empty");

        var pollingTime = configuration.GetSection("QueueUrl:PollingTime").Get<int>();

        if (pollingTime <= 0)
            throw new ArgumentException("QueueUrl:PollingTime is empty");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Polling the queue to get the newest message first
                var message = await handler.GetMessageAsync(queueUrl, pollingTime, stoppingToken);

                if (message.Event is null || message.ReceiptHandle is null)
                {
                    // Block the queueing of job and continue to listen
                    continue;
                }

                var eventType = message.Event.Type;

                // The enqueued job deletes the SQS message itself once its work succeeds
                switch (eventType)
                {
                    case Models.Enums.EventType.Create:
                    case Models.Enums.EventType.Update:
                        jobEnqueuer.Enqueue(processor => processor.ProcessIndexAsync(message.Event.ToDispatchModel(), queueUrl, message.ReceiptHandle, CancellationToken.None));
                        break;

                    case Models.Enums.EventType.Delete:
                        jobEnqueuer.Enqueue(processor => processor.ProcessDeleteAsync(message.Event.DispatchId, queueUrl, message.ReceiptHandle, CancellationToken.None));
                        break;
                }
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
                logger.LogError("Unexpected error when polling {QueueURL}. Details: {Error}", queueUrl, exception.InnerException);
            }
        }

        // return Task.CompletedTask;
    }
}
