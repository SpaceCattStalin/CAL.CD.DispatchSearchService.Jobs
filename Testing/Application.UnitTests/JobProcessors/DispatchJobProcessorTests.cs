using Moq;
using SearchJobs.Api;
using SearchJobs.Api.Interfaces;
using SearchJobs.Api.JobProcessors.DispatchQueueProcessor;
using SearchJobs.Api.Models;

namespace Application.UnitTests;

public class DispatchJobProcessorTests
{
    private readonly Mock<IDispatchSearchServiceClient> _searchClient = new();
    private readonly Mock<IDispatchServiceMessagesHandler> _messagesHandler = new();
    private readonly DispatchJobProcessor _sut;

    public DispatchJobProcessorTests()
    {
        _sut = new DispatchJobProcessor(_searchClient.Object, _messagesHandler.Object);
    }

    [Fact]
    public async Task ProcessIndexAsync_IndexesDispatchThenDeletesMessage()
    {
        var dispatch = new DispatchModel { DispatchId = Guid.NewGuid() };

        await _sut.ProcessIndexAsync(dispatch, "queue-url", "receipt-1", CancellationToken.None);

        _searchClient.Verify(c => c.IndexAsync(dispatch, It.IsAny<CancellationToken>()), Times.Once);
        _messagesHandler.Verify(m => m.DeleteMessageAsync("queue-url", "receipt-1"), Times.Once);
    }

    [Fact]
    public async Task ProcessDeleteAsync_DeletesDispatchThenDeletesMessage()
    {
        var dispatchId = Guid.NewGuid();

        await _sut.ProcessDeleteAsync(dispatchId, "queue-url", "receipt-2", CancellationToken.None);

        _searchClient.Verify(c => c.DeleteAsync(dispatchId, It.IsAny<CancellationToken>()), Times.Once);
        _messagesHandler.Verify(m => m.DeleteMessageAsync("queue-url", "receipt-2"), Times.Once);
    }

    [Fact]
    public async Task ProcessUpdateAsync_UpdatesDispatchThenDeletesMessage()
    {
        var dispatch = new DispatchModel { DispatchId = Guid.NewGuid() };

        await _sut.ProcessUpdateAsync(dispatch, "queue-url", "receipt-3", CancellationToken.None);

        _searchClient.Verify(c => c.UpdateAsync(dispatch, It.IsAny<CancellationToken>()), Times.Once);
        _messagesHandler.Verify(m => m.DeleteMessageAsync("queue-url", "receipt-3"), Times.Once);
    }

    [Fact]
    public async Task ProcessIndexAsync_SearchClientThrows_DoesNotDeleteMessage()
    {
        // If indexing fails, the SQS message must NOT be deleted so the message is
        // redelivered and retried - deleting it here would silently drop the dispatch.
        var dispatch = new DispatchModel { DispatchId = Guid.NewGuid() };
        _searchClient
            .Setup(c => c.IndexAsync(dispatch, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("search service unavailable"));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.ProcessIndexAsync(dispatch, "queue-url", "receipt-1", CancellationToken.None));

        _messagesHandler.Verify(m => m.DeleteMessageAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
