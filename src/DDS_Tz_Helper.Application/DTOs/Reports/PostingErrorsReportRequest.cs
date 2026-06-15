namespace DDS_Tz_Helper.Application.DTOs.Reports;

public class PostingErrorsReportRequest
{
    public DateTime fromDate { get; set; }
    public DateTime toDate { get; set; }
    public int user_id { get; set; }
}
