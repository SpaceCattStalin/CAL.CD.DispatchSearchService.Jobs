namespace SearchJobs.Api.Models;

public static class DispatchWriterDtoExtensions
{
    public static DispatchModel ToDispatchModel(this DispatchWriterDto dispatchDto)
    {
        return new DispatchModel
        {
            DispatchId = dispatchDto.DispatchId,
            PriceTotal = (double)dispatchDto.PriceTotal,
            PickupDate = dispatchDto.PickupDate,
            DropoffDate = dispatchDto.DropoffDate,
            DispatchStatus = dispatchDto.DispatchStatus.ToString(),
            Vehicles = dispatchDto.Vehicles
                .Select(v => new VehicleModel { Vin = v.Vin ?? string.Empty })
                .ToList()
        };
    }
}
