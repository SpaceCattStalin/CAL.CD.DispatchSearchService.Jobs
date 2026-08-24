namespace SearchJobs.Api;

public interface IBackfillJob
{
    Task RunAsync();
}
