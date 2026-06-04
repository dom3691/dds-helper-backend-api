namespace DDS_Tz_Helper.Application.DTOs.TripMaintenance;

public class AddDriverManuallyRequest
{
    public int? driver_no { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public string plant { get; set; }
    public bool third_party { get; set; }
    public int user_id { get; set; }
}
