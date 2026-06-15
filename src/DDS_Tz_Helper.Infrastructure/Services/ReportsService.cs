using DDS_Tz_Helper.Application.DTOs.BusinessObjects;
using DDS_Tz_Helper.Application.DTOs.Reports;
using DDS_Tz_Helper.Application.DTOs.Results;
using DDS_Tz_Helper.Application.Interfaces;
using DDS_Tz_Helper.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DDS_Tz_Helper.Infrastructure.Services;

public class ReportsService : IReportsService
{
    private const string SelectPlaceholder = "-- select --";
    private readonly IDispatchDBContext _context;
    private readonly ITransactionLogService _transactionLogService;

    public ReportsService(IDispatchDBContext context, ITransactionLogService transactionLogService)
    {
        _context = context;
        _transactionLogService = transactionLogService;
    }

    public async Task<IReadOnlyList<gateaccess_info>> GetReportDataFilterAsync(GateReportFilterRequest request, CancellationToken cancellationToken = default)
    {
        var query = _context.gateaccess_info
            .AsNoTracking()
            .Where(a => a.entry_datetime >= request.fromDate && a.entry_datetime <= request.toDate);

        if (!string.IsNullOrWhiteSpace(request.product))
        {
            query = query.Where(a => a.fleet_no == request.product);
        }

        if (!IsSelectPlaceholder(request.truckStatus))
        {
            query = query.Where(a => a.status == request.truckStatus);
        }

        if (!IsSelectPlaceholder(request.trxType))
        {
            query = query.Where(a => a.transaction_type == request.trxType);
        }

        await RecordReportLogAsync(request.user_id, "GATE_REPORT_SPOOL_HELPER", "GATE REPORT SPOOL: ", cancellationToken);
        return await query.OrderBy(s => s.entry_datetime).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Truck_Analysis_Material_only_Result>> GetMaterialReportAsync(MaterialReportRequest request, CancellationToken cancellationToken = default)
    {
        var report = await _context.Truck_Analysis_Material_only_Results
            .FromSqlInterpolated($"EXEC dbo.Truck_Analysis_Material_only @datefrom={request.fromDate}, @dateto={request.toDate}")
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.product))
        {
            report = report.Where(a => a.PRODUCT == request.product).ToList();
        }

        await RecordReportLogAsync(request.user_id, "MATERIAL_REPORT_SPOOL_HELPER", "MATERIAL REPORT SPOOL HELPER: ", cancellationToken);
        return report;
    }

    public async Task<IReadOnlyList<trip_registry>> GetTripCreationDetailsReportAsync(TripDetailsReportRequest request, CancellationToken cancellationToken = default)
    {
        var query = _context.trip_registry
            .AsNoTracking()
            .Where(a => a.creation_datetime >= request.fromDate
                && a.creation_datetime <= request.toDate
                && a.trip_no.StartsWith("38"));

        if (!string.IsNullOrWhiteSpace(request.Vehicle))
        {
            query = query.Where(a => a.fleet == request.Vehicle);
        }

        if (!IsSelectPlaceholder(request.truckStatus))
        {
            query = query.Where(a => a.tms_status == request.truckStatus);
        }

        if (!string.IsNullOrWhiteSpace(request.driver))
        {
            query = query.Where(a => a.driver_id_sap == request.driver);
        }

        await RecordReportLogAsync(request.user_id, "TRIP_CREATE_REPORT_SPOOL_HELPER", "TRIP REPORT SPOOL: ", cancellationToken);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<view_closure_dto>> GetTripClosureDetailsReportAsync(TripDetailsReportRequest request, CancellationToken cancellationToken = default)
    {
        var tripClosures = _context.trip_closure
            .AsNoTracking()
            .Where(a => a.closure_date >= request.fromDate && a.closure_date <= request.toDate && a.customer_no != "SAP");

        var sapTripMatches = await (
            from closure in tripClosures
            join trip in _context.trip_registry.AsNoTracking() on closure.trip_id equals trip.trip_id_sap
            select new TripClosureReportJoin(closure, trip))
            .ToListAsync(cancellationToken);

        var tripNumberMatches = await (
            from closure in tripClosures
            join trip in _context.trip_registry.AsNoTracking() on closure.trip_id equals trip.trip_no
            select new TripClosureReportJoin(closure, trip))
            .ToListAsync(cancellationToken);

        var report = sapTripMatches.Concat(tripNumberMatches)
            .Select(match => CreateClosureDto(match.Closure, match.Trip))
            .OrderByDescending(s => s.closure_date)
            .ToList();

        await RecordReportLogAsync(request.user_id, "TRIP_CLOSURE_REPORT_SPOOL_HELPER", "TRIP REPORT SPOOL: ", cancellationToken);
        return report;
    }

    public async Task<IReadOnlyList<transaction_data>> GetReportPostingErrorsAsync(PostingErrorsReportRequest request, CancellationToken cancellationToken = default)
    {
        var report = await _context.transaction_data
            .AsNoTracking()
            .Where(a => a.gross_time >= request.fromDate
                && a.gross_time <= request.toDate
                && a.error_field != "OFFLINE"
                && a.sync_status == false
                && a.error_field != " ")
            .ToListAsync(cancellationToken);

        await RecordReportLogAsync(request.user_id, "POSTING_ERRORS_REPORT_SPOOL_HELPER", "POSTING ERRORS REPORT SPOOL: ", cancellationToken);
        return report;
    }

    public async Task<IReadOnlyList<sto_transaction_data>> GetReportSTOPostingErrorsAsync(PostingErrorsReportRequest request, CancellationToken cancellationToken = default)
    {
        var report = await _context.sto_transaction_data
            .AsNoTracking()
            .Where(a => a.gross_time >= request.fromDate
                && a.gross_time <= request.toDate
                && a.error_field != "OFFLINE"
                && a.sync_status == false
                && a.error_field != " ")
            .ToListAsync(cancellationToken);

        await RecordReportLogAsync(request.user_id, "POSTING_ERRORS_REPORT_SPOOL_HELPER", "STO POSTING ERRORS REPORT SPOOL: ", cancellationToken);
        return report;
    }

    private static view_closure_dto CreateClosureDto(trip_closure closure, trip_registry trip)
    {
        return new view_closure_dto
        {
            trip_id = closure.trip_id,
            waybill = closure.waybill,
            closure_date = closure.closure_date,
            damaged_qty = closure.damaged_qty.ToString(),
            caked_qty = closure.caked_qty.ToString(),
            short_qty = closure.short_qty.ToString(),
            miss_reason = closure.miss_reason,
            driver_id_sap = trip.driver_id_sap.ToString(),
            driver_name = trip.driver_name,
            atc_no = trip.atc_no,
            trip_id_sap = trip.trip_id_sap,
            atc_type = trip.atc_type,
            fleet = trip.fleet,
            migo_details = trip.migo_details
        };
    }

    private async Task RecordReportLogAsync(int userId, string activity, string fieldName, CancellationToken cancellationToken)
    {
        if (userId <= 0)
        {
            return;
        }

        await _transactionLogService.RecordLogAsync("0000000000", userId, activity, fieldName, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static bool IsSelectPlaceholder(string value)
    {
        return string.IsNullOrWhiteSpace(value) || value == SelectPlaceholder;
    }

    private sealed record TripClosureReportJoin(trip_closure Closure, trip_registry Trip);
}
