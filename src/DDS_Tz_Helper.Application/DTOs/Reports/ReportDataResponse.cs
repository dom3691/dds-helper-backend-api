namespace DDS_Tz_Helper.Application.DTOs.Reports;

public class ReportDataResponse<T>
{
    public IReadOnlyList<T> data { get; set; } = [];
}
