using Microsoft.Extensions.Options;
using SearchJobs.Api.Models;

namespace SearchJobs.Api;

public class DispatchSyncJob(
    IDispatchServiceClient dispatchServiceClient,
    IDispatchSearchServiceClient dispatchSearchServiceClient,
    ICheckpointStore checkpointStore,
    ILogger<DispatchSyncJob> logger,
    IOptions<AppSettings> appSettings) : ISyncJob
{
    private const string JobName = nameof(DispatchSyncJob);
    private readonly string _apiKey = appSettings.Value.DispatchService.ApiKey!;
    public async Task RunAsync()
    {
        var cursor = await checkpointStore.GetLastCursorAsync(JobName);

        logger.LogInformation("{JobName} starting from cursor '{Cursor}'.", JobName, cursor);

        var totalProcessed = 0;

        while (true)
        {
            PageResponseWithCursor<DispatchWriterDto> page = await dispatchServiceClient.GetAsync(cursor, _apiKey);
            var dispatchModels = page.Items.Select(dto => dto.ToDispatchModel()).ToList();

            if (dispatchModels.Count > 0)
            {
                await dispatchSearchServiceClient.BatchUpsertAsync(dispatchModels);
                totalProcessed += dispatchModels.Count;
            }

            if (string.IsNullOrEmpty(page.Cursor))
                break;

            cursor = page.Cursor;

            await checkpointStore.SaveCursorAsync(JobName, cursor);

            logger.LogInformation("{JobName} checkpointed at cursor '{Cursor}', {Total} dispatches processed so far.", JobName, cursor, totalProcessed);
        }

        await checkpointStore.SaveCursorAsync(JobName, string.Empty);

        logger.LogInformation("{JobName} completed, {Total} dispatches processed.", JobName, totalProcessed);
    }
}
