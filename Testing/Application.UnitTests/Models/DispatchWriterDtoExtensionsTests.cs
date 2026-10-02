using SearchJobs.Api.Models;
using SearchJobs.Api.Models.Enums;

namespace Application.UnitTests;

public class DispatchWriterDtoExtensionsTests
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
        var dto = new DispatchWriterDto(
            dispatchId, carrierId, shipperId, 123.45m,
            pickupDate, dropoffDate, DispatchStatus.Delivered,
            [new DispatchWriterVehicle("VIN1")], createdAt);

        var model = dto.ToDispatchModel();

        Assert.Equal(dispatchId, model.DispatchId);
        Assert.Equal(carrierId, model.CarrierId);
        Assert.Equal(shipperId, model.ShipperId);
        Assert.Equal(123.45, model.PriceTotal);
        Assert.Equal(pickupDate, model.PickupDate);
        Assert.Equal(dropoffDate, model.DropoffDate);
        Assert.Equal("Delivered", model.DispatchStatus);
        Assert.Equal(createdAt, model.CreatedAt);
        var vehicle = Assert.Single(model.Vehicles);
        Assert.Equal("VIN1", vehicle.Vin);
    }

    [Fact]
    public void ToDispatchModel_VehicleWithNullVin_MapsToEmptyString()
    {
        var dto = new DispatchWriterDto(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 0m,
            DateTime.UtcNow, DateTime.UtcNow, DispatchStatus.PendingPickup,
            [new DispatchWriterVehicle(null)], DateTime.UtcNow);

        var model = dto.ToDispatchModel();

        var vehicle = Assert.Single(model.Vehicles);
        Assert.Equal(string.Empty, vehicle.Vin);
    }
}
