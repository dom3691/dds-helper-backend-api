namespace DDS_Tz_Helper.Application.DTOs.Reports;

public class TripDetailsReportRequest
{
    public DateTime fromDate { get; set; }
    public DateTime toDate { get; set; }
    public string trxType { get; set; }
    public string driver { get; set; }
    public string truckStatus { get; set; }
    public string Vehicle { get; set; }
    public string createdBy { get; set; }
    public int user_id { get; set; }
}
