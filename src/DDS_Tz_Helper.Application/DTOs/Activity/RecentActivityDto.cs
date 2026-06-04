namespace DDS_Tz_Helper.Application.DTOs.Activity;

public class RecentActivityDto
{
    public DateTime? time { get; set; }
    public string activity { get; set; }
    public string reference { get; set; }
    public string plant { get; set; }
    public string user { get; set; }
    public string status { get; set; }
}
