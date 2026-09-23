namespace SearchJobs.Api;

public interface ICheckpointStore
{
    Task<string> GetLastCursorAsync(string jobName);
    Task SaveCursorAsync(string jobName, string cursor);
}
