using Microsoft.AspNetCore.Mvc;

namespace SearchJobs.Api;

[ApiController]
[Route("api/jobs")]
public class JobsController(IJobEnqueuer<ISyncJob> jobEnqueuer) : ControllerBase
{
    [HttpPost("sync")]
    public IActionResult TriggerBackfill()
    {
        var authHeader = Request.Headers.Authorization.ToString();
        // var jobId = jobEnqueuer.Enqueue(job => job.RunAsync());
        var jobId = jobEnqueuer.Enqueue(job => job.RunAsync(authHeader));

        return Accepted(new { JobId = jobId });
    }
}
