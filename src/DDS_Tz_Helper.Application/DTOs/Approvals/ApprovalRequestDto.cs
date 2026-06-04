namespace DDS_Tz_Helper.Application.DTOs.Approvals;

public class ApprovalRequestDto
{
    public int id { get; set; }
    public string requestType { get; set; }
    public string requestTitle { get; set; }
    public string atc { get; set; }
    public string currentValue { get; set; }
    public string requestedValue { get; set; }
    public string tripId { get; set; }
    public string reason { get; set; }
    public string requestedBy { get; set; }
    public DateTime? requestedOn { get; set; }
    public string status { get; set; }
}
