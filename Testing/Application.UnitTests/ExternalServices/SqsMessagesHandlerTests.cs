using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Moq;
using SearchJobs.Api;
using SearchJobs.Api.Models;
using SearchJobs.Api.Models.Enums;

namespace Application.UnitTests;

public class SqsMessagesHandlerTests
{
    private readonly Mock<IAmazonSQS> _sqsClient = new();
    private readonly SqsMessagesHandler _sut;

    public SqsMessagesHandlerTests()
    {
        _sut = new SqsMessagesHandler(_sqsClient.Object, Mock.Of<ILogger<SqsMessagesHandler>>());
    }

    [Fact]
    public async Task GetMessageAsync_QueueEmpty_ReturnsNullEventAndNullReceiptHandle()
    {
        _sqsClient
            .Setup(c => c.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReceiveMessageResponse { Messages = [] });

        var (@event, receiptHandle) = await _sut.GetMessageAsync("queue-url", 5, CancellationToken.None);

        Assert.Null(@event);
        Assert.Null(receiptHandle);
    }

    [Fact]
    public async Task GetMessageAsync_MessageAvailable_DeserializesEventAndReturnsReceiptHandle()
    {
        var dispatchId = Guid.NewGuid();
        var writerEvent = new DispatchWriterEvent(
            EventType.Create, dispatchId, Guid.NewGuid(), Guid.NewGuid(), 100m,
            DateTime.UtcNow, DateTime.UtcNow.AddDays(1), DispatchStatus.PendingPickup,
            [], DateTime.UtcNow);
        var body = JsonSerializer.Serialize(writerEvent);
        _sqsClient
            .Setup(c => c.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReceiveMessageResponse
            {
                Messages = [new Message { Body = body, ReceiptHandle = "receipt-1" }]
            });

        var (@event, receiptHandle) = await _sut.GetMessageAsync("queue-url", 5, CancellationToken.None);

        Assert.NotNull(@event);
        Assert.Equal(dispatchId, @event!.DispatchId);
        Assert.Equal("receipt-1", receiptHandle);
    }

    [Fact]
    public async Task GetMessageAsync_PassesQueueUrlAndWaitTimeToSqsClient()
    {
        _sqsClient
            .Setup(c => c.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReceiveMessageResponse { Messages = [] });

        await _sut.GetMessageAsync("queue-url", 20, CancellationToken.None);

        _sqsClient.Verify(c => c.ReceiveMessageAsync(
            It.Is<ReceiveMessageRequest>(r => r.QueueUrl == "queue-url" && r.WaitTimeSeconds == 20 && r.MaxNumberOfMessages == 1),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteMessageAsync_SendsDeleteRequestWithQueueUrlAndReceiptHandle()
    {
        _sqsClient
            .Setup(c => c.DeleteMessageAsync(It.IsAny<DeleteMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteMessageResponse());

        await _sut.DeleteMessageAsync("queue-url", "receipt-1");

        _sqsClient.Verify(c => c.DeleteMessageAsync(
            It.Is<DeleteMessageRequest>(r => r.QueueUrl == "queue-url" && r.ReceiptHandle == "receipt-1"),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
