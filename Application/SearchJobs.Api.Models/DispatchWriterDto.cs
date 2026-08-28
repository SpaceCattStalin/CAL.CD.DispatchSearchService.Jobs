using SearchJobs.Api.Models.Enums;

namespace SearchJobs.Api.Models;

public class DispatchWriterDto(
    Guid DispatchId,
    decimal PriceTotal,
    DateTime PickupDate,
    DateTime DropoffDate,
    DispatchStatus DispatchStatus,
    IEnumerable<DispatchWriterVehicle> Vehicles)
{
    public Guid DispatchId { get; } = DispatchId;
    public decimal PriceTotal { get; } = PriceTotal;
    public DateTime PickupDate { get; } = PickupDate;
    public DateTime DropoffDate { get; } = DropoffDate;
    public DispatchStatus DispatchStatus { get; } = DispatchStatus;
    public IEnumerable<DispatchWriterVehicle> Vehicles { get; } = Vehicles;
}
