using SearchJobs.Api.Models.Enums;

namespace SearchJobs.Api.Models;

public class DispatchWriterEvent(
    EventType Type,
    Guid DispatchId,
    Guid CarrierId,
    Guid ShipperId,
    decimal PriceTotal,
    DateTime PickupDate,
    DateTime DropoffDate,
    DispatchStatus DispatchStatus,
    IEnumerable<DispatchWriterVehicle> Vehicles,
    DateTime CreatedAt)
{
    public EventType Type { get; } = Type;
    public Guid DispatchId { get; } = DispatchId;
    public Guid CarrierId { get; set; } = CarrierId;
    public Guid ShipperId { get; set; } = ShipperId;
    public decimal PriceTotal { get; } = PriceTotal;
    public DateTime PickupDate { get; } = PickupDate;
    public DateTime DropoffDate { get; } = DropoffDate;
    public DispatchStatus DispatchStatus { get; } = DispatchStatus;
    public IEnumerable<DispatchWriterVehicle> Vehicles { get; } = Vehicles;
    public DateTime CreatedAt { get; } = CreatedAt;
}

public class DispatchWriterVehicle(string? Vin)
{
    public string? Vin { get; } = Vin;
}
