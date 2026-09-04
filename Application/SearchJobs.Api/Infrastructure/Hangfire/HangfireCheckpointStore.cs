using Hangfire;

namespace SearchJobs.Api;

public class HangfireCheckpointStore(JobStorage jobStorage) : ICheckpointStore
{
    private const string HashKeyPrefix = "checkpoint:";
    private const string CursorField = "cursor";

    public Task<string> GetLastCursorAsync(string jobName)
    {
        using var connection = jobStorage.GetConnection();
        var entries = connection.GetAllEntriesFromHash(HashKeyPrefix + jobName);

        var cursor = entries != null && entries.TryGetValue(CursorField, out var value)
            ? value
            : string.Empty;

        return Task.FromResult(cursor);
    }

    public Task SaveCursorAsync(string jobName, string cursor)
    {
        using var connection = jobStorage.GetConnection();
        connection.SetRangeInHash(HashKeyPrefix + jobName, new Dictionary<string, string> { [CursorField] = cursor });

        return Task.CompletedTask;
    }
}
