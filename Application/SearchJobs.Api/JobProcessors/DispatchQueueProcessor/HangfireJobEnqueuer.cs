using System.Linq.Expressions;
using Hangfire;
using Hangfire.Common;
using SearchJobs.Api.Models;

namespace SearchJobs.Api.JobProcessors.DispatchQueueProcessor;

public class HangfireJobEnqueuer<T>(IBackgroundJobClient hangfireClient, ILogger<HangfireJobEnqueuer<T>> logger) : IJobEnqueuer<T>
{
    public string Enqueue(Expression<Func<T, Task>> job)
    {
        var jobId = hangfireClient.Enqueue(job);
        return jobId;
    }
}
