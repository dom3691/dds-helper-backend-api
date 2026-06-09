using DDS_Tz_Helper.Application.DTOs.BusinessObjects;
using DDS_Tz_Helper.Application.DTOs.TripMaintenance;
using DDS_Tz_Helper.Application.Interfaces;
using DDS_Tz_Helper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace DDS_Tz_Helper.Infrastructure.Services;

public class TripMaintenanceService : ITripMaintenanceService
{
    private static readonly Regex NamePattern = new("^[A-Za-z ]+$", RegexOptions.Compiled);

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

        var tradeDetails = await _context.Trade.FirstOrDefaultAsync(x => x.sparenum2 == atcEntered, cancellationToken);
        var atcDetails = await _context.atcs.FirstOrDefaultAsync(x => x.sales_doc_number == request.atc, cancellationToken);
        var transDataDetails = await _context.transaction_data.FirstOrDefaultAsync(x => x.sales_doc_number == request.atc && x.sync_status != true, cancellationToken);
        var weighedOut = await _context.transaction_data.FirstOrDefaultAsync(a => a.sales_doc_number == request.atc && a.gross_time != null, cancellationToken);

        if (weighedOut != null)
        {
            atcResponse.msg = "ATC has been USED and WEIGHED-OUT!!!";
            atcResponse.status = false;
            return atcResponse;
        }

        if (tradeDetails != null)
        {
            var pending = ApprovalService.CreatePendingRequest(ApprovalService.UpdateDriverRequest, request.atc, request.user_id);
            pending.driver = request.driver;

            _context.transaction_data_archive.Add(pending);
            await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "UPDATE_DRIVER_HELPER", "UPDATE_DRIVER_HELPER : ", cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            tradeResponse.msg = "Driver update request submitted successfully; approval is pending.";
            tradeResponse.status = true;
            return tradeResponse;
        }

        if (transDataDetails != null)
        {
            var pending = ApprovalService.CreatePendingRequest(ApprovalService.UpdateDriverRequest, request.atc, request.user_id);
            pending.driver = request.driver;

            _context.transaction_data_archive.Add(pending);
            await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "UPDATE_DRIVER_HELPER", "UPDATE_DRIVER_HELPER : ", cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            transactionResponse.msg = "Driver update request submitted successfully; approval is pending.";
            transactionResponse.status = true;
            return transactionResponse;
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

        var tradeDetails = await _context.Trade.FirstOrDefaultAsync(x => x.sparenum2 == atcEntered, cancellationToken);
        var transDataDetails = await _context.transaction_data.FirstOrDefaultAsync(x => x.sales_doc_number == request.atc && x.gross_time == null, cancellationToken);

        if (tradeDetails != null)
        {
            var pending = ApprovalService.CreatePendingRequest(ApprovalService.UpdateDestinationRequest, request.atc, request.user_id);
            pending.destination = request.destination;

            _context.transaction_data_archive.Add(pending);
            await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "UPDATE_DESTINATION_HELPER", "UPDATE_DESTINATION_HELPER: ", cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            tradeResponse.msg = "Destination update request submitted successfully; approval is pending.";
            tradeResponse.status = true;
            return tradeResponse;
        }

        if (transDataDetails != null)
        {
            var pending = ApprovalService.CreatePendingRequest(ApprovalService.UpdateDestinationRequest, request.atc, request.user_id);
            pending.destination = request.destination;

            _context.transaction_data_archive.Add(pending);
            await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "UPDATE_DESTINATION_HELPER", "UPDATE_DESTINATION_HELPER: ", cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            transactionResponse.msg = "Destination update request submitted successfully; approval is pending.";
            transactionResponse.status = true;
            return transactionResponse;
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

        if (tripRegistryDetails == null)
        {
            response.msg = "WRONG ATC ID ENTERED";
            response.status = false;
            return response;
        }

        var pending = ApprovalService.CreatePendingRequest(ApprovalService.UpdateTripRequest, request.trip, request.user_id);

        _context.transaction_data_archive.Add(pending);
        await _transactionLogService.RecordLogAsync(request.trip, request.user_id, "UPDATE_TRIP_HELPER", "UPDATE_TRIP_HELPER : ", cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        response.msg = "Trip validation request submitted successfully; approval is pending.";
        response.status = true;
        return response;
    }

    public async Task<trip_registry_dto> AddOnlineTripAsync(AddOnlineTripRequest request, CancellationToken cancellationToken = default)
    {
        var response = new trip_registry_dto();

        if (request is null || string.IsNullOrWhiteSpace(request.atc) || string.IsNullOrWhiteSpace(request.trip))
        {
            response.msg = "ATC number and Online Trip ID are required.";
            response.status = false;
            return response;
        }

        if (!Regex.IsMatch(request.trip.Trim(), @"^\d{1,10}$"))
        {
            response.msg = "Online Trip ID must contain only numbers and must not be more than 10 digits.";
            response.status = false;
            return response;
        }

        var tripRegistryDetails = await _context.trip_registry
            .FirstOrDefaultAsync(x => x.tms_online_status == null && x.atc_no == request.atc, cancellationToken);

        if (tripRegistryDetails == null)
        {
            response.msg = "WRONG ATC NUMBER ENTERED";
            response.status = false;
            return response;
        }

        var onlineTripPending = ApprovalService.CreatePendingRequest(ApprovalService.AddOnlineTripRequest, request.atc, request.user_id);
        onlineTripPending.trip_id = request.trip.Trim();

        _context.transaction_data_archive.Add(onlineTripPending);
        await _transactionLogService.RecordLogAsync(request.atc, request.user_id, "UPDATE_ONLINE_TRIP_HELPER", "UPDATE_ONLINE_TRIP_HELPER: ", cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        response.msg = "Online trip request submitted successfully; approval is pending.";
        response.status = true;
        return response;
    }

    public async Task<trip_registry_dto> AddDriverManuallyAsync(AddDriverManuallyRequest request, CancellationToken cancellationToken = default)
    {
        var response = new trip_registry_dto();

        if (request is null || request.driver_no is null || string.IsNullOrWhiteSpace(request.first_name) || string.IsNullOrWhiteSpace(request.last_name))
        {
            response.msg = "Driver number, first name, and last name are required.";
            response.status = false;
            return response;
        }

        if (request.driver_no <= 0)
        {
            response.msg = "Driver number must be a valid integer.";
            response.status = false;
            return response;
        }

        var firstName = request.first_name.Trim();
        var lastName = request.last_name.Trim();
        if (!NamePattern.IsMatch(firstName) || !NamePattern.IsMatch(lastName))
        {
            response.msg = "First name and last name must contain letters only. Special characters and numbers are not allowed.";
            response.status = false;
            return response;
        }

        var driverNo = request.driver_no.Value.ToString();
        var exists = await _context.driver.AnyAsync(x => x.driver_no == driverNo, cancellationToken);
        if (exists)
        {
            response.msg = "Driver number already exists.";
            response.status = false;
            return response;
        }

        var pending = ApprovalService.CreatePendingRequest(ApprovalService.AddDriverManuallyRequest, driverNo, request.user_id);
        pending.driver = firstName;
        pending.transporter = lastName;
        pending.loc = request.plant?.Trim();
        pending.code_a = request.third_party.ToString().ToLowerInvariant();

        _context.transaction_data_archive.Add(pending);
        await _transactionLogService.RecordLogAsync(driverNo, request.user_id, "ADD_DRIVER_MANUALLY_REQUEST_HELPER", $"{firstName} {lastName}", cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        response.msg = "Driver add request submitted successfully; approval is pending.";
        response.status = true;
        return response;
    }

}

