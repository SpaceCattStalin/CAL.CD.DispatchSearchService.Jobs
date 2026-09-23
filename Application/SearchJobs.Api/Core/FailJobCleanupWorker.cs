using System;
using Hangfire;
using Hangfire.States;
using Hangfire.Storage.Monitoring;

namespace SearchJobs.Api.Core;
/// <summary>
/// Cron job to clean up fail background job
///  See <see href="https://github.com/HangfireIO/Hangfire/issues/2156"/> for context.
/// </summary>
/// <param name="logger"></param>
public class FailJobCleanupWorker(ILogger<FailJobCleanupWorker> logger)
{
    public void Execute(IJobCancellationToken token)
    {
        using (logger.BeginScope(new Dictionary<string, object>{
            { "labels.messageType", "FailJobsCleanupWorker"},
        }))
        {
            var api = JobStorage.Current.GetMonitoringApi();
            JobList<FailedJobDto> failJobs;
            logger.LogInformation("Executing Failed Hangfire Jobs Cleanup Job.");
            try
            {
                failJobs = api.FailedJobs(0, 1000);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Could not get fail jobs, Reason . {ex.Message}");
                return;
            }

            if (failJobs.Any())
            {
                Parallel.ForEach(failJobs, (failJob, state) =>
                {
                    MarkJobAsDeleted(state, failJob, token);
                });

                logger.LogInformation("Succesfully deleted Failed jobs in Hangfire.");
            }
            else
            {
                logger.LogInformation("Could not find Failed jobs in Hangfire to Delete.");
            }
        }
    }


    public void MarkJobAsDeleted(ParallelLoopState state, KeyValuePair<string, FailedJobDto> failJob, IJobCancellationToken token)
    {
        if (token.ShutdownToken.IsCancellationRequested)
        {
            state.Break();
            return;
        }
        if (failJob.Value != null)
        {
            var jobId = failJob.Key;
            try
            {
                if (DateTime.UtcNow - failJob.Value.FailedAt > TimeSpan.FromSeconds(10))
                {
                    BackgroundJob.Delete(jobId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Exception when delete fail job in hangfire", ex);
            }
        }
    }
}
