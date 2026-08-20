using Amazon.SQS;
using Amazon.SQS.Model;
using SearchJobs.Api.Interfaces;
using SearchJobs.Api.Models;
using System.Text.Json;

namespace SearchJobs.Api;

public class SqsMessagesHandler(IAmazonSQS sqsClient, ILogger<SqsMessagesHandler> logger)
                                                                            : IMessagesHandler
{
    public async Task<DispatchWriterEvent> GetMessageAsync(string queueUrl, int waitTime, CancellationToken stoppingToken)
    {
        var response = await sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
        {
            QueueUrl = queueUrl,
            WaitTimeSeconds = waitTime,
            MaxNumberOfMessages = 1
        });

        var message = response.Messages.FirstOrDefault();
        var dispatchWriterEvent = JsonSerializer.Deserialize<DispatchWriterEvent>(message.Body);
        logger.LogInformation("Received SQS message {MessageId}: {@DispatchWriterEvent}", message.MessageId, dispatchWriterEvent);
        
        return dispatchWriterEvent!;
    }
}
