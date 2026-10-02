using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SearchJobs.Api;

[ApiController]
[Route("api/jobs")]
public class JobsController(IJobEnqueuer<ISyncJob> jobEnqueuer) : ControllerBase
{
    [HttpPost("sync")]
    [Authorize(Policy = "sync:update-all")]
    public IActionResult TriggerSync()
    {
        var jobId = jobEnqueuer.Enqueue(job => job.RunAsync());

        return Accepted(new { JobId = jobId });
    }
}
