namespace SearchJobs.Api;

public interface ISyncJob
{
    Task RunAsync();
    Task RunAsync(string auth);
}
