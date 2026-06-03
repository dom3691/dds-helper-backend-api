using DDS_Tz_Helper.Application.DTOs.BusinessObjects;
using DDS_Tz_Helper.Application.DTOs.TripMaintenance;
using DDS_Tz_Helper.Application.Interfaces;
using DDS_Tz_Helper.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DDS_Tz_Helper.Infrastructure.Services;

public class TripMaintenanceService : ITripMaintenanceService
{
    private readonly IDispatchDBContext _context;
    private readonly ITransactionLogService _transactionLogService;

    public TripMaintenanceService(IDispatchDBContext context, ITransactionLogService transactionLogService)
    {
        _context = context;
        _transactionLogService = transactionLogService;
    }

    public async Task<object> DoUpdateDriverAsync(DoUpdateDriverRequest request, CancellationToken cancellationToken = default)
    {
        var tradeResponse = new trade_dto();
        var transactionResponse = new transaction_dto();
        var atcResponse = new atc_datax();
        decimal.TryParse(request?.atc, out var atcEntered);

        var tradeDetails = await _context.Trades.FirstOrDefaultAsync(x => x.sparenum2 == atcEntered, cancellationToken);
        var atcDetails = await _context.atcs.FirstOrDefaultAsync(x => x.sales_doc_number == request.atc, cancellationToken);
        var transDataDetails = await _context.transaction_data.FirstOrDefaultAsync(x => x.sales_doc_number == request.atc && x.sync_status != true, cancellationToken);
        var weighedOut = await _context.transaction_data.FirstOrDefaultAsync(a => a.sales_doc_number == request.atc && a.gross_time != null, cancellationToken);

        if (weighedOut != null)
        {
            atcResponse.msg = "ATC has been USED and WEIGHED-OUT!!!";
            atcResponse.status = false;
            return atcResponse;
        }

        try
        {
            if (tradeDetails != null)
            {
                tradeDetails.sparestr1 = request.driver;
                tradeDetails.sparestr5 = request.driver;

                if (atcDetails != null)
                {
                    atcDetails.drivers_name = request.driver;
                }

                await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "UPDATE_DRIVER_HELPER", "UPDATE_DRIVER_HELPER : ", cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                tradeResponse.msg = "Driver Updated Successfully";
                tradeResponse.status = true;
                return tradeResponse;
            }

            if (transDataDetails != null)
            {
                transDataDetails.driver = request.driver;
                await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "UPDATE_DRIVER_HELPER", "UPDATE_DRIVER_HELPER : ", cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                transactionResponse.msg = "Driver Updated Successfully";
                transactionResponse.status = true;
                return transactionResponse;
            }
        }
        catch
        {
            tradeResponse.msg = "Something went wrong, plese contact I.T";
            tradeResponse.status = false;
            return tradeResponse;
        }

        tradeResponse.msg = "ATC not found, please ensure ATC is correct";
        tradeResponse.status = false;
        return tradeResponse;
    }

    public async Task<object> DoUpdateDestinationAsync(DoUpdateDestinationRequest request, CancellationToken cancellationToken = default)
    {
        var tradeResponse = new trade_dto();
        var transactionResponse = new transaction_dto();
        decimal.TryParse(request?.atc, out var atcEntered);

        var tradeDetails = await _context.Trades.FirstOrDefaultAsync(x => x.sparenum2 == atcEntered, cancellationToken);
        var transDataDetails = await _context.transaction_data.FirstOrDefaultAsync(x => x.sales_doc_number == request.atc && x.gross_time == null, cancellationToken);

        try
        {
            if (tradeDetails != null)
            {
                tradeDetails.sparestr2 = request.destination;
                tradeDetails.receiver = request.destination;
                tradeDetails.receivercode = request.destination;
                await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "UPDATE_DESTINATION_HELPER", "UPDATE_DESTINATION_HELPER: ", cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                tradeResponse.msg = "Destination Updated Successfully";
                tradeResponse.status = true;
                return tradeResponse;
            }

            if (transDataDetails != null)
            {
                transDataDetails.destination = request.destination;
                await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "UPDATE_DESTINATION_HELPER", "UPDATE_DESTINATION_HELPER: ", cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                transactionResponse.msg = "Destination Updated Successfully";
                transactionResponse.status = true;
                return transactionResponse;
            }
        }
        catch
        {
            tradeResponse.msg = "Something went wrong, plese contact I.T";
            tradeResponse.status = false;
            return tradeResponse;
        }

        tradeResponse.msg = "ATC not found, please ensure ATC is correct";
        tradeResponse.status = false;
        return tradeResponse;
    }

    public async Task<trip_registry_dto> DoUpdateTripAsync(DoUpdateTripRequest request, CancellationToken cancellationToken = default)
    {
        var response = new trip_registry_dto();
        var tripRegistryDetails = await _context.trip_registry
            .FirstOrDefaultAsync(x => (x.tms_online_status == "FUEL_POSTED" || x.tms_online_status == "CLOSED")
                && (x.tms_status == "FUEL_POSTED" || x.tms_online_status == "CLOSED")
                && x.atc_no == request.trip, cancellationToken);

        try
        {
            if (tripRegistryDetails == null)
            {
                response.msg = "WRONG ATC ID ENTERED";
                response.status = false;
                return response;
            }

            tripRegistryDetails.tms_status = "APPROVED";
            tripRegistryDetails.tms_online_status = "APPROVED";
            await _transactionLogService.RecordLogAsync(request.trip, request.user_id, "UPDATE_TRIP_HELPER", "UPDATE_TRIP_HELPER : ", cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            response.msg = "TRIP VALIDATED";
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

    public async Task<trip_registry_dto> AddOnlineTripAsync(AddOnlineTripRequest request, CancellationToken cancellationToken = default)
    {
        var response = new trip_registry_dto();
        var tripRegistryDetails = await _context.trip_registry
            .FirstOrDefaultAsync(x => x.tms_online_status == null && x.atc_no == request.atc, cancellationToken);

        try
        {
            if (tripRegistryDetails == null)
            {
                response.msg = "WRONG ATC NUMBER ENTERED";
                response.status = false;
                return response;
            }

            tripRegistryDetails.tms_online_status = "APPROVED";
            tripRegistryDetails.trip_id_sap = request.trip;
            await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "UPDATE_ONLINE_TRIP_HELPER", "UPDATE_ONLINE_TRIP_HELPER: ", cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            response.msg = "ONLINE TRIP ID ADDED";
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

