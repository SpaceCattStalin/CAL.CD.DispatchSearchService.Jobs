using SearchJobs.Api.Models.Enums;

namespace SearchJobs.Api.Models;

public record class DispatchWriterDto(
    Guid DispatchId,
    decimal PriceTotal,
    DateTime PickupDate,
    DateTime DropoffDate,
    DispatchStatus DispatchStatus,
    IEnumerable<DispatchWriterVehicle> Vehicles);
