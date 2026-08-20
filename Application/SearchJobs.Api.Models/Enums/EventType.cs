using System.Text.Json.Serialization;

namespace SearchJobs.Api.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EventType
{
    Create,
    Delete,
    Update
}
