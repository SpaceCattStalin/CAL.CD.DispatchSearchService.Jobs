using Amazon.SQS;
using Amazon.SQS.Model;
using SearchJobs.Api.Interfaces;
using SearchJobs.Api.Models;
using System.Text.Json;

namespace SearchJobs.Api;

public class SqsMessagesHandler(IAmazonSQS sqsClient, ILogger<SqsMessagesHandler> logger)
                                                                            : IMessagesHandler
{

    /// <summary>
    /// Call to delete message in queue after the message is finish processing (response status code is 200)
    /// </summary>
    /// <param name="queueUrl"></param>
    /// <param name="receiptHandle"></param>
    /// <returns></returns>
    public async Task DeleteMessageAsync(string queueUrl, string receiptHandle)
    {
        await sqsClient.DeleteMessageAsync(new DeleteMessageRequest
        {
            QueueUrl = queueUrl,
            ReceiptHandle = receiptHandle
        });
    }
    /// <summary>
    /// Call to get a message from the queue
    /// </summary>
    /// <param name="queueUrl"></param>
    /// <param name="waitTime"></param>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    public async Task<(DispatchWriterEvent? Event, string? ReceiptHandle)> GetMessageAsync(string queueUrl, int waitTime, CancellationToken stoppingToken)
    {
        var response = await sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
        {
            QueueUrl = queueUrl,
            WaitTimeSeconds = waitTime,
            MaxNumberOfMessages = 1
        });

        var message = response.Messages?.FirstOrDefault();

        if (message is null) return (null, null);

        var dispatchWriterEvent = JsonSerializer.Deserialize<DispatchWriterEvent>(message.Body);
        logger.LogInformation("Received SQS message {MessageId}: {@DispatchWriterEvent}", message.MessageId, dispatchWriterEvent);

        return (dispatchWriterEvent, message.ReceiptHandle);
    }
}
