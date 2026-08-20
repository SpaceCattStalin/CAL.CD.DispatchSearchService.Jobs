using SearchJobs.Api.Models;

namespace SearchJobs.Api;

public interface IJobEnqueuer
{
    public string Enqueue(DispatchWriterEvent dispatchEvent);
}
