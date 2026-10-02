using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SearchJobs.Api;
using SearchJobs.Api.Models;
using SearchJobs.Api.Models.Enums;

namespace Application.UnitTests;

public class DispatchSyncJobTests
{
    private readonly Mock<IDispatchServiceClient> _dispatchServiceClient = new();
    private readonly Mock<IDispatchSearchServiceClient> _dispatchSearchServiceClient = new();
    private readonly Mock<ICheckpointStore> _checkpointStore = new();
    private readonly DispatchSyncJob _sut;

    public DispatchSyncJobTests()
    {
        _sut = new DispatchSyncJob(
            _dispatchServiceClient.Object,
            _dispatchSearchServiceClient.Object,
            _checkpointStore.Object,
            Mock.Of<ILogger<DispatchSyncJob>>(),
            Options());

        _checkpointStore.Setup(c => c.GetLastCursorAsync(It.IsAny<string>())).ReturnsAsync(string.Empty);
    }

    private static IOptions<AppSettings> Options() =>
        Microsoft.Extensions.Options.Options.Create(new AppSettings
        {
            QueueUrl = new QueueUrlSettings { PrimaryQueue = "queue-url", PollingTime = 5 },
            ConnectionStrings = new ConnectionStringsSettings { HangfireDb = "test-connection" },
            SearchService = new ServiceSettings { BaseUrl = "https://search.test" },
            DispatchService = new ServiceSettings { BaseUrl = "https://dispatch.test", ApiKey = "test-api-key" }
        });

    private static DispatchWriterDto CreateDto() => new(
        Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 100m,
        DateTime.UtcNow, DateTime.UtcNow.AddDays(1), DispatchStatus.PendingPickup,
        [], DateTime.UtcNow);

    [Fact]
    public async Task RunAsync_StartsFromLastSavedCursor()
    {
        _checkpointStore.Setup(c => c.GetLastCursorAsync(nameof(DispatchSyncJob))).ReturnsAsync("saved-cursor");
        _dispatchServiceClient
            .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(new PageResponseWithCursor<DispatchWriterDto>([], null));

        await _sut.RunAsync();

        _dispatchServiceClient.Verify(c => c.GetAsync("saved-cursor", "test-api-key", 500), Times.Once);
    }

    [Fact]
    public async Task RunAsync_EmptyPage_DoesNotCallBatchUpsert()
    {
        _dispatchServiceClient
            .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(new PageResponseWithCursor<DispatchWriterDto>([], null));

        await _sut.RunAsync();

        _dispatchSearchServiceClient.Verify(c => c.BatchUpsertAsync(It.IsAny<List<DispatchModel>>()), Times.Never);
    }

    [Fact]
    public async Task RunAsync_PageWithItems_UpsertsMappedModels()
    {
        var dto = CreateDto();
        _dispatchServiceClient
            .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(new PageResponseWithCursor<DispatchWriterDto>([dto], null));

        await _sut.RunAsync();

        _dispatchSearchServiceClient.Verify(
            c => c.BatchUpsertAsync(It.Is<List<DispatchModel>>(models => models.Count == 1 && models[0].DispatchId == dto.DispatchId)),
            Times.Once);
    }

    [Fact]
    public async Task RunAsync_MultiplePages_PaginatesUntilCursorIsEmptyAndCheckpointsBetweenPages()
    {
        var firstDto = CreateDto();
        var secondDto = CreateDto();
        _dispatchServiceClient.SetupSequence(c => c.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(new PageResponseWithCursor<DispatchWriterDto>([firstDto], "page-2-cursor"))
            .ReturnsAsync(new PageResponseWithCursor<DispatchWriterDto>([secondDto], null));

        await _sut.RunAsync();

        _dispatchServiceClient.Verify(c => c.GetAsync(string.Empty, It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        _dispatchServiceClient.Verify(c => c.GetAsync("page-2-cursor", It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        _checkpointStore.Verify(c => c.SaveCursorAsync(nameof(DispatchSyncJob), "page-2-cursor"), Times.Once);
        _dispatchSearchServiceClient.Verify(c => c.BatchUpsertAsync(It.IsAny<List<DispatchModel>>()), Times.Exactly(2));
    }

    [Fact]
    public async Task RunAsync_Completes_SavesEmptyCursorAsFinalCheckpoint()
    {
        _dispatchServiceClient
            .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(new PageResponseWithCursor<DispatchWriterDto>([], null));

        await _sut.RunAsync();

        _checkpointStore.Verify(c => c.SaveCursorAsync(nameof(DispatchSyncJob), string.Empty), Times.Once);
    }
}
