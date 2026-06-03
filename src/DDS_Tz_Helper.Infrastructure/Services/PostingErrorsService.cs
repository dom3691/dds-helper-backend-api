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

        try
        {
            if (offlineTrip == null)
            {
                response.msg = "WRONG ATC ID ENTERED or ATC NOT SHOWING 'OFFLINE'!!!";
                response.status = false;
                return response;
            }

            offlineTrip.error_field = "Repush";
            await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "UPDATE_OFFLINE_TRIP_for_REPUSH_HELPER", "UPDATE_OFFLINE_TRIP_for_REPUSH_HELPER : ", cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            response.msg = "REPUSH ATC FROM POSTING-ERRORS PAGE";
            response.status = true;
            return response;
        }
        catch
        {
            response.msg = "Something went wrong, plese contact I.T";
            response.status = false;
            return response;
        }
    }

    public async Task<trip_registry_dto> DoUpdateMolassesAsync(AtcPostingErrorRequest request, CancellationToken cancellationToken = default)
    {
        var response = new trip_registry_dto();
        var molassesDelivery = await _context.transaction_data.FirstOrDefaultAsync(x => x.sync_status == false && x.sales_doc_number == request.atc, cancellationToken);

        try
        {
            if (molassesDelivery == null)
            {
                response.msg = "WRONG ATC ID ENTERED or ATC IS ALREADY SYNCED !!!";
                response.status = false;
                return response;
            }

            molassesDelivery.sales_order_item = molassesDelivery.sales_order_item == "FORTIFIED SUGAR" ? "MOLASSES" : "FORTIFIED SUGAR";
            await _context.SaveChangesAsync(cancellationToken);

            response.msg = "PRODUCT NAME CHANGED";
            response.status = true;
            return response;
        }
        catch
        {
            response.msg = "Something went wrong, plese contact I.T";
            response.status = false;
            return response;
        }
    }

    public async Task<trip_registry_dto> DoRemoveTripIDForCTESAsync(AtcPostingErrorRequest request, CancellationToken cancellationToken = default)
    {
        var response = new trip_registry_dto();
        var offlineTrip = await _context.transaction_data.FirstOrDefaultAsync(x => x.sync_status == false && x.gross != null && x.gross_time != null && x.sales_doc_number == request.atc && x.error_field == "Invalid Trip ID and Delivery combination", cancellationToken);

        try
        {
            if (offlineTrip == null)
            {
                response.msg = "WRONG ATC ID ENTERED or ATC NOT WEIGHED OUT or NO INVALID COMBINATION!!!";
                response.status = false;
                return response;
            }

            var archive = new transaction_data_archive
            {
                trip_id = offlineTrip.trip_id,
                sales_doc_number = offlineTrip.sales_doc_number,
                message_field = "archived after changing trip via DDS HELPER"
            };

            _context.transaction_data_archive.Add(archive);
            await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "REMOVE_CTES_TRIP_HELPER", "REMOVE_CTES_TRIP_HELPER: ", cancellationToken);

            offlineTrip.error_field = "Repush";
            offlineTrip.trip_id = "0000000000";
            await _context.SaveChangesAsync(cancellationToken);

            response.msg = "REPUSH ATC FROM POSTING-ERRORS PAGE";
            response.status = true;
            return response;
        }
        catch
        {
            response.msg = "Something went wrong, plese contact I.T";
            response.status = false;
            return response;
        }
    }
}
