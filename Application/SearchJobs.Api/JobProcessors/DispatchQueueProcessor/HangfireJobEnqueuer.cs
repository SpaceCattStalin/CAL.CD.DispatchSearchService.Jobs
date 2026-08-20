using SearchJobs.Api.Models;

namespace SearchJobs.Api.JobProcessors.DispatchQueueProcessor;

public class HangfireJobEnqueuer : IJobEnqueuer
{
    public string Enqueue(DispatchWriterEvent dispatchEvent)
    {
        throw new NotImplementedException();
    }
}
