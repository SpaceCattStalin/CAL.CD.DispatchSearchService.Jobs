using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace SearchJobs.Api;

public class AppSettings
{
    [Required]
    public required QueueUrlSettings QueueUrl { get; init; }

    [Required]
    public required ConnectionStringsSettings ConnectionStrings { get; init; }

    [Required]
    public required ServiceSettings SearchService { get; init; }

    [Required]
    public required ServiceSettings DispatchService { get; init; }
}

public class QueueUrlSettings
{
    [Required]
    public required string PrimaryQueue { get; init; }

    [Range(1, int.MaxValue)]
    public int PollingTime { get; init; }
}

public class ConnectionStringsSettings
{
    [Required]
    public required string HangfireDb { get; init; }
}

public class ServiceSettings
{
    [Required]
    public required string BaseUrl { get; init; }
    public string? ApiKey { get; init; }
}
