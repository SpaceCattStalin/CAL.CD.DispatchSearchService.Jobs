using SearchJobs.Api.Models;
using SearchJobs.Api.Models.Enums;

namespace Application.UnitTests;

public class DispatchWriterEventExtensionsTests
{
    [Fact]
    public void ToDispatchModel_MapsAllFields()
    {
        var dispatchId = Guid.NewGuid();
        var carrierId = Guid.NewGuid();
        var shipperId = Guid.NewGuid();
        var pickupDate = DateTime.UtcNow;
        var dropoffDate = pickupDate.AddDays(1);
        var createdAt = DateTime.UtcNow.AddHours(-1);
        var writerEvent = new DispatchWriterEvent(
            EventType.Create, dispatchId, carrierId, shipperId, 200.50m,
            pickupDate, dropoffDate, DispatchStatus.PendingDelivery,
            [new DispatchWriterVehicle("VIN2")], createdAt);

        var model = writerEvent.ToDispatchModel();

        Assert.Equal(dispatchId, model.DispatchId);
        Assert.Equal(carrierId, model.CarrierId);
        Assert.Equal(shipperId, model.ShipperId);
        Assert.Equal(200.50, model.PriceTotal);
        Assert.Equal(pickupDate, model.PickupDate);
        Assert.Equal(dropoffDate, model.DropoffDate);
        Assert.Equal("PendingDelivery", model.DispatchStatus);
        Assert.Equal(createdAt, model.CreatedAt);
        var vehicle = Assert.Single(model.Vehicles);
        Assert.Equal("VIN2", vehicle.Vin);
    }

    [Fact]
    public void ToDispatchModel_VehicleWithNullVin_MapsToEmptyString()
    {
        var writerEvent = new DispatchWriterEvent(
            EventType.Delete, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 0m,
            DateTime.UtcNow, DateTime.UtcNow, DispatchStatus.Canceled,
            [new DispatchWriterVehicle(null)], DateTime.UtcNow);

        var model = writerEvent.ToDispatchModel();

        var vehicle = Assert.Single(model.Vehicles);
        Assert.Equal(string.Empty, vehicle.Vin);
    }
}
