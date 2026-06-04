namespace DDS_Tz_Helper.Application.DTOs.Approvals;

public class ApprovalActionRequest
{
    public int requestId { get; set; }
    public int user_id { get; set; }
    public string comment { get; set; }
}
