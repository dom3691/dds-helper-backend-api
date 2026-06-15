namespace DDS_Tz_Helper.Application.DTOs.Reports;

public class MaterialReportRequest
{
    public DateTime fromDate { get; set; }
    public DateTime toDate { get; set; }
    public string product { get; set; }
    public int user_id { get; set; }
}
