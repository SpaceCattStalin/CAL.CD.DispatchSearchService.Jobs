using Microsoft.AspNetCore.Mvc;

namespace SearchJobs.Api;

[ApiController]
[Route("api/jobs")]
public class JobsController(IJobEnqueuer<ISyncJob> jobEnqueuer) : ControllerBase
{
    [HttpPost("sync")]
    public IActionResult TriggerBackfill()
    {
        var jobId = jobEnqueuer.Enqueue(job => job.RunAsync());

        return Accepted(new { JobId = jobId });
    }
}
