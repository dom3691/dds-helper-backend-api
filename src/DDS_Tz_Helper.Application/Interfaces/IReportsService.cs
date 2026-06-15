using DDS_Tz_Helper.Application.DTOs.BusinessObjects;
using DDS_Tz_Helper.Application.DTOs.Reports;
using DDS_Tz_Helper.Application.DTOs.Results;
using DDS_Tz_Helper.Domain.Entities;

namespace DDS_Tz_Helper.Application.Interfaces;

public interface IReportsService
{
    Task<IReadOnlyList<gateaccess_info>> GetReportDataFilterAsync(GateReportFilterRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Truck_Analysis_Material_only_Result>> GetMaterialReportAsync(MaterialReportRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<trip_registry>> GetTripCreationDetailsReportAsync(TripDetailsReportRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<view_closure_dto>> GetTripClosureDetailsReportAsync(TripDetailsReportRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<transaction_data>> GetReportPostingErrorsAsync(PostingErrorsReportRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<sto_transaction_data>> GetReportSTOPostingErrorsAsync(PostingErrorsReportRequest request, CancellationToken cancellationToken = default);
}
