using System.Text.Json.Serialization;

namespace SearchJobs.Api.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DispatchStatus
{
    NotSigned,
    PendingPickup,
    PendingDelivery,
    Delivered,
    Canceled
}
