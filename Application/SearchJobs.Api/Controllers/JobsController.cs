using Microsoft.AspNetCore.Mvc;

namespace SearchJobs.Api;

[ApiController]
[Route("jobs")]
public class JobsController(IJobEnqueuer<IBackfillJob> jobEnqueuer) : ControllerBase
{
    [HttpPost("backfill")]
    public IActionResult TriggerBackfill()
    {
        var jobId = jobEnqueuer.Enqueue(job => job.RunAsync());

        return Accepted(new { JobId = jobId });
    }
}
