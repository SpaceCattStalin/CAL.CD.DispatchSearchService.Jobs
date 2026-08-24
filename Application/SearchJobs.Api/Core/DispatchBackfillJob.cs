using SearchJobs.Api.Models;

namespace SearchJobs.Api;

public class DispatchBackfillJob(
    IDispatchServiceClient dispatchServiceClient,
    IDispatchSearchServiceClient dispatchSearchServiceClient,
    ICheckpointStore checkpointStore,
    ILogger<DispatchBackfillJob> logger) : IBackfillJob
{
    private const string JobName = nameof(DispatchBackfillJob);

    public async Task RunAsync()
    {
        var cursor = await checkpointStore.GetLastCursorAsync(JobName);

        logger.LogInformation("{JobName} starting from cursor '{Cursor}'.", JobName, cursor);

        var totalProcessed = 0;

        while (true)
        {
            var page = await dispatchServiceClient.GetAsync(cursor);
            var dispatchModels = page.Items.Select(dto => dto.ToDispatchModel()).ToList();

            if (dispatchModels.Count > 0)
            {
                await dispatchSearchServiceClient.BatchUpsertAsync(dispatchModels);
                totalProcessed += dispatchModels.Count;
            }

            if (string.IsNullOrEmpty(page.Cursor))
                break;

            cursor = page.Cursor;

            // Checkpoint only after the current page is successfully upserted, so a crash mid-run
            // resumes from the last fully-processed page instead of re-walking from the start.
            await checkpointStore.SaveCursorAsync(JobName, cursor);

            logger.LogInformation("{JobName} checkpointed at cursor '{Cursor}', {Total} dispatches processed so far.", JobName, cursor, totalProcessed);
        }

        // Full run completed — clear the checkpoint so the next manual trigger starts from the beginning again.
        await checkpointStore.SaveCursorAsync(JobName, string.Empty);

        logger.LogInformation("{JobName} completed, {Total} dispatches processed.", JobName, totalProcessed);
    }
}
