using SearchJobs.Api.Models.Enums;

namespace SearchJobs.Api.Models;

public record class DispatchWriterEvent(EventType Type, Guid DispatchId);
