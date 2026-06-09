using DDS_Tz_Helper.Application.DTOs.BusinessObjects;
using DDS_Tz_Helper.Application.DTOs.PostingErrors;
using DDS_Tz_Helper.Application.Interfaces;
using DDS_Tz_Helper.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DDS_Tz_Helper.Infrastructure.Services;

public class PostingErrorsService : IPostingErrorsService
{
    private readonly IDispatchDBContext _context;
    private readonly ITransactionLogService _transactionLogService;

    public PostingErrorsService(IDispatchDBContext context, ITransactionLogService transactionLogService)
    {
        _context = context;
        _transactionLogService = transactionLogService;
    }

    public async Task<trip_registry_dto> DoUpdateOfflineTripTForRepushAsync(AtcPostingErrorRequest request, CancellationToken cancellationToken = default)
    {
        var response = new trip_registry_dto();
        var offlineTrip = await _context.transaction_data.FirstOrDefaultAsync(x => x.sync_status == false && x.gross != null && x.gross_time != null && x.sales_doc_number == request.atc && x.error_field == "OFFLINE", cancellationToken);

        if (offlineTrip == null)
        {
            response.msg = "WRONG ATC ID ENTERED or ATC NOT SHOWING 'OFFLINE'!!!";
            response.status = false;
            return response;
        }

        var pending = ApprovalService.CreatePendingRequest(ApprovalService.RepushOfflineRequest, request.atc, request.user_id);

        _context.transaction_data_archive.Add(pending);
        await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "UPDATE_OFFLINE_TRIP_for_REPUSH_HELPER", "UPDATE_OFFLINE_TRIP_for_REPUSH_HELPER : ", cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        response.msg = "Offline repush request submitted successfully; approval is pending.";
        response.status = true;
        return response;
    }

    public async Task<trip_registry_dto> DoUpdateMolassesAsync(AtcPostingErrorRequest request, CancellationToken cancellationToken = default)
    {
        var response = new trip_registry_dto();
        var molassesDelivery = await _context.transaction_data.FirstOrDefaultAsync(x => x.sync_status == false && x.sales_doc_number == request.atc, cancellationToken);

        if (molassesDelivery == null)
        {
            response.msg = "WRONG ATC ID ENTERED or ATC IS ALREADY SYNCED !!!";
            response.status = false;
            return response;
        }

        var molassesPending = ApprovalService.CreatePendingRequest(ApprovalService.UpdateMolassesRequest, request.atc, request.user_id);
        molassesPending.seal = molassesDelivery.sales_order_item == "FORTIFIED SUGAR" ? "MOLASSES" : "FORTIFIED SUGAR";

        _context.transaction_data_archive.Add(molassesPending);
        await _context.SaveChangesAsync(cancellationToken);

        response.msg = "Molasses update request submitted successfully; approval is pending.";
        response.status = true;
        return response;
    }

    public async Task<trip_registry_dto> DoRemoveTripIDForCTESAsync(AtcPostingErrorRequest request, CancellationToken cancellationToken = default)
    {
        var response = new trip_registry_dto();
        var offlineTrip = await _context.transaction_data.FirstOrDefaultAsync(x => x.sync_status == false && x.gross != null && x.gross_time != null && x.sales_doc_number == request.atc && x.error_field == "Invalid Trip ID and Delivery combination", cancellationToken);

        if (offlineTrip == null)
        {
            response.msg = "WRONG ATC ID ENTERED or ATC NOT WEIGHED OUT or NO INVALID COMBINATION!!!";
            response.status = false;
            return response;
        }

        var archive = ApprovalService.CreatePendingRequest(ApprovalService.RemoveCtesTripRequest, request.atc, request.user_id);
        archive.trip_id = offlineTrip.trip_id;
        archive.message_field = "archived after changing trip via DDS HELPER";

        _context.transaction_data_archive.Add(archive);
        await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "REMOVE_CTES_TRIP_HELPER", "REMOVE_CTES_TRIP_HELPER: ", cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        response.msg = "CTES trip removal request submitted successfully; approval is pending.";
        response.status = true;
        return response;
    }
}
