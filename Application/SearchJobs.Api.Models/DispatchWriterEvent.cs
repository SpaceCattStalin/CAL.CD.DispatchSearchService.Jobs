using SearchJobs.Api.Models.Enums;

namespace SearchJobs.Api.Models;

public record class DispatchWriterEvent(
    EventType Type,
    Guid DispatchId,
    decimal PriceTotal,
    DateTime PickupDate,
    DateTime DropoffDate,
    DispatchStatus DispatchStatus,
    IEnumerable<DispatchWriterVehicle> Vehicles);

public record class DispatchWriterVehicle(string? Vin);
