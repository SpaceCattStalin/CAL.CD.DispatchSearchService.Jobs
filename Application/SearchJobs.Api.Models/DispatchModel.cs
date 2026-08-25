namespace SearchJobs.Api.Models;

public class DispatchModel
{
    public Guid DispatchId { get; set; } = Guid.Empty;
    public double PriceTotal { get; set; }
    public DateTime PickupDate { get; set; }
    public DateTime DropoffDate { get; set; }
    public string DispatchStatus { get; set; } = string.Empty;
    public List<VehicleModel> Vehicles { get; set; } = [];

    // TODO: remove after debug
    public override string ToString()
    {
        return $"DispatchId={DispatchId}, PriceTotal={PriceTotal}, PickupDate={PickupDate}, DropoffDate={DropoffDate}, DispatchStatus={DispatchStatus}, Vehicles={Vehicles.Count}";
    }
}
