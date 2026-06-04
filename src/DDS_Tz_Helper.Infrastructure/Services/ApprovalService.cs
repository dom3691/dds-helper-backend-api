using DDS_Tz_Helper.Application.DTOs.Approvals;
using DDS_Tz_Helper.Application.DTOs.BusinessObjects;
using DDS_Tz_Helper.Application.Interfaces;
using DDS_Tz_Helper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DDS_Tz_Helper.Infrastructure.Services;

public class ApprovalService : IApprovalService
{
    public const string PendingStatus = "PENDING";
    public const string ApprovedStatus = "APPROVED";
    public const string RejectedStatus = "REJECTED";

    public const string AtcSwapRequest = "ATC SWAP REQUEST";
    public const string UpdateDriverRequest = "UPDATE DRIVER REQUEST";
    public const string UpdateDestinationRequest = "UPDATE DESTINATION REQUEST";
    public const string UpdateReceiverRequest = "UPDATE RECEIVER REQUEST";
    public const string ChangeGradeRequest = "CHANGE ATC GRADE REQUEST";
    public const string UpdateTripRequest = "UPDATE TRIP REQUEST";
    public const string AddOnlineTripRequest = "ADD ONLINE TRIP REQUEST";
    public const string RepushOfflineRequest = "REPUSH OFFLINE TRIP REQUEST";
    public const string UpdateMolassesRequest = "UPDATE MOLASSES REQUEST";
    public const string RemoveCtesTripRequest = "REMOVE CTES TRIP REQUEST";
    public const string AddDriverManuallyRequest = "ADD DRIVER MANUALLY REQUEST";

    private static readonly string[] SupportedRequestTypes =
    [
        AtcSwapRequest,
        UpdateDriverRequest,
        UpdateDestinationRequest,
        UpdateReceiverRequest,
        ChangeGradeRequest,
        UpdateTripRequest,
        AddOnlineTripRequest,
        RepushOfflineRequest,
        UpdateMolassesRequest,
        RemoveCtesTripRequest,
        AddDriverManuallyRequest
    ];

    private readonly IDispatchDBContext _context;
    private readonly IConfiguration _configuration;
    private readonly ITransactionLogService _transactionLogService;

    public ApprovalService(IDispatchDBContext context, IConfiguration configuration, ITransactionLogService transactionLogService)
    {
        _context = context;
        _configuration = configuration;
        _transactionLogService = transactionLogService;
    }

    public async Task<IReadOnlyList<ApprovalRequestDto>> GetPendingApprovalsAsync(int userId, CancellationToken cancellationToken = default)
    {
        if (!await CanApproveAsync(userId, cancellationToken))
        {
            return Array.Empty<ApprovalRequestDto>();
        }

        var pending = await _context.transaction_data_archive
            .Where(x => x.sales_doc_type == PendingStatus && SupportedRequestTypes.Contains(x.operator_weighin))
            .OrderByDescending(x => x.tare_time ?? x.picking_time ?? x.gross_time)
            .ToListAsync(cancellationToken);

        return pending.Select(MapApproval).ToList();
    }

    public async Task<trip_registry_dto> ApproveAsync(ApprovalActionRequest request, CancellationToken cancellationToken = default)
    {
        var response = new trip_registry_dto();

        if (request is null || request.requestId <= 0)
        {
            return Failure(response, "Invalid approval request.");
        }

        if (!await CanApproveAsync(request.user_id, cancellationToken))
        {
            return Failure(response, "You are not allowed to approve requests.");
        }

        var approval = await _context.transaction_data_archive
            .FirstOrDefaultAsync(x => x.id == request.requestId && x.sales_doc_type == PendingStatus, cancellationToken);

        if (approval is null)
        {
            return Failure(response, "This request is no longer pending.");
        }

        var applyResponse = await ApplyApprovedChangeAsync(approval, request.user_id, cancellationToken);
        if (!applyResponse.status)
        {
            return applyResponse;
        }

        approval.operator_picking = request.comment;
        approval.sales_doc_type = ApprovedStatus;
        approval.picking_time = DateTime.Now;
        approval.wb_in = request.user_id.ToString();

        await _transactionLogService.RecordLogAsync(approval.sales_doc_number ?? "N/A", request.user_id, $"APPROVED_{NormalizeActivity(approval.operator_weighin)}", request.comment ?? string.Empty, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        response.msg = "Request approved successfully.";
        response.status = true;
        return response;
    }

    public async Task<trip_registry_dto> DeclineAsync(ApprovalActionRequest request, CancellationToken cancellationToken = default)
    {
        var response = new trip_registry_dto();

        if (request is null || request.requestId <= 0)
        {
            return Failure(response, "Invalid approval request.");
        }

        if (!await CanApproveAsync(request.user_id, cancellationToken))
        {
            return Failure(response, "You are not allowed to decline requests.");
        }

        var approval = await _context.transaction_data_archive
            .FirstOrDefaultAsync(x => x.id == request.requestId && x.sales_doc_type == PendingStatus, cancellationToken);

        if (approval is null)
        {
            return Failure(response, "This request is no longer pending.");
        }

        approval.operator_picking = request.comment;
        approval.sales_doc_type = RejectedStatus;
        approval.picking_time = DateTime.Now;
        approval.wb_in = request.user_id.ToString();

        await _transactionLogService.RecordLogAsync(approval.sales_doc_number ?? "N/A", request.user_id, $"DECLINED_{NormalizeActivity(approval.operator_weighin)}", request.comment ?? string.Empty, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        response.msg = "Request declined successfully.";
        response.status = true;
        return response;
    }

    public static transaction_data_archive CreatePendingRequest(string requestType, string atc, int userId)
    {
        return new transaction_data_archive
        {
            sales_doc_number = atc,
            shp_point = userId.ToString(),
            operator_weighin = requestType,
            sales_doc_type = PendingStatus,
            tare_time = DateTime.Now
        };
    }

    private async Task<trip_registry_dto> ApplyApprovedChangeAsync(transaction_data_archive approval, int approverId, CancellationToken cancellationToken)
    {
        return approval.operator_weighin switch
        {
            AtcSwapRequest => await ApproveAtcSwapAsync(approval, cancellationToken),
            UpdateDriverRequest => await ApproveDriverUpdateAsync(approval, cancellationToken),
            UpdateDestinationRequest => await ApproveDestinationUpdateAsync(approval, cancellationToken),
            UpdateReceiverRequest => await ApproveReceiverUpdateAsync(approval, cancellationToken),
            ChangeGradeRequest => await ApproveGradeChangeAsync(approval, cancellationToken),
            UpdateTripRequest => await ApproveTripUpdateAsync(approval, cancellationToken),
            AddOnlineTripRequest => await ApproveOnlineTripAsync(approval, cancellationToken),
            RepushOfflineRequest => await ApproveOfflineRepushAsync(approval, cancellationToken),
            UpdateMolassesRequest => await ApproveMolassesUpdateAsync(approval, cancellationToken),
            RemoveCtesTripRequest => await ApproveCtesTripRemovalAsync(approval, cancellationToken),
            AddDriverManuallyRequest => await ApproveManualDriverAddAsync(approval, cancellationToken),
            _ => Failure(new trip_registry_dto(), "Unsupported approval request type.")
        };
    }

    private async Task<trip_registry_dto> ApproveAtcSwapAsync(transaction_data_archive approval, CancellationToken cancellationToken)
    {
        var oldAtc = approval.vehicle;
        var newAtc = approval.sales_doc_number;
        var transaction = await _context.transaction_data.FirstOrDefaultAsync(x => x.sales_doc_number == oldAtc && x.gross == null, cancellationToken);
        if (transaction is null)
        {
            return Failure(new trip_registry_dto(), "Original ATC was not found or has already been weighed out.");
        }

        transaction.sales_doc_number = newAtc;
        transaction.trip_id = approval.trip_id;

        var oldAtcData = await _context.atc_data.FirstOrDefaultAsync(x => x.sales_doc_number == oldAtc, cancellationToken);
        if (oldAtcData is not null)
        {
            oldAtcData.used = null;
        }

        var newAtcData = await _context.atc_data.FirstOrDefaultAsync(x => x.sales_doc_number == newAtc, cancellationToken);
        if (newAtcData is not null)
        {
            newAtcData.used = true;
        }

        return Success("ATC swap approved.");
    }

    private async Task<trip_registry_dto> ApproveDriverUpdateAsync(transaction_data_archive approval, CancellationToken cancellationToken)
    {
        decimal.TryParse(approval.sales_doc_number, out var atcEntered);
        var tradeDetails = await _context.Trades.FirstOrDefaultAsync(x => x.sparenum2 == atcEntered, cancellationToken);
        var atcDetails = await _context.atcs.FirstOrDefaultAsync(x => x.sales_doc_number == approval.sales_doc_number, cancellationToken);
        var transDataDetails = await _context.transaction_data.FirstOrDefaultAsync(x => x.sales_doc_number == approval.sales_doc_number && x.sync_status != true, cancellationToken);

        if (tradeDetails is not null)
        {
            tradeDetails.sparestr1 = approval.driver;
            tradeDetails.sparestr5 = approval.driver;
            if (atcDetails is not null)
            {
                atcDetails.drivers_name = approval.driver;
            }

            return Success("Driver update approved.");
        }

        if (transDataDetails is not null)
        {
            transDataDetails.driver = approval.driver;
            return Success("Driver update approved.");
        }

        return Failure(new trip_registry_dto(), "ATC was not found for driver update.");
    }

    private async Task<trip_registry_dto> ApproveDestinationUpdateAsync(transaction_data_archive approval, CancellationToken cancellationToken)
    {
        decimal.TryParse(approval.sales_doc_number, out var atcEntered);
        var tradeDetails = await _context.Trades.FirstOrDefaultAsync(x => x.sparenum2 == atcEntered, cancellationToken);
        var transDataDetails = await _context.transaction_data.FirstOrDefaultAsync(x => x.sales_doc_number == approval.sales_doc_number && x.gross_time == null, cancellationToken);

        if (tradeDetails is not null)
        {
            tradeDetails.sparestr2 = approval.destination;
            tradeDetails.receiver = approval.destination;
            tradeDetails.receivercode = approval.destination;
            return Success("Destination update approved.");
        }

        if (transDataDetails is not null)
        {
            transDataDetails.destination = approval.destination;
            return Success("Destination update approved.");
        }

        return Failure(new trip_registry_dto(), "ATC was not found for destination update.");
    }

    private async Task<trip_registry_dto> ApproveReceiverUpdateAsync(transaction_data_archive approval, CancellationToken cancellationToken)
    {
        var atcDetails = await _context.atc_data.FirstOrDefaultAsync(x => x.sales_doc_number == approval.sales_doc_number && x.used == null, cancellationToken);
        if (atcDetails is null)
        {
            return Failure(new trip_registry_dto(), "ATC was not found or has already been used.");
        }

        atcDetails.customer_name = approval.sender;
        return Success("Receiver update approved.");
    }

    private async Task<trip_registry_dto> ApproveGradeChangeAsync(transaction_data_archive approval, CancellationToken cancellationToken)
    {
        var atcDetails = await _context.atc_data.FirstOrDefaultAsync(x => x.sales_doc_number == approval.sales_doc_number && x.item_number != null, cancellationToken);
        if (atcDetails is null)
        {
            return Failure(new trip_registry_dto(), "ATC was not found for grade change.");
        }

        atcDetails.sales_order_item = approval.sales_order_item;
        return Success("ATC grade change approved.");
    }

    private async Task<trip_registry_dto> ApproveTripUpdateAsync(transaction_data_archive approval, CancellationToken cancellationToken)
    {
        var trip = await _context.trip_registry.FirstOrDefaultAsync(x => (x.tms_online_status == "FUEL_POSTED" || x.tms_online_status == "CLOSED")
            && (x.tms_status == "FUEL_POSTED" || x.tms_online_status == "CLOSED")
            && x.atc_no == approval.sales_doc_number, cancellationToken);

        if (trip is null)
        {
            return Failure(new trip_registry_dto(), "Trip was not found or is not ready for validation.");
        }

        trip.tms_status = ApprovedStatus;
        trip.tms_online_status = ApprovedStatus;
        return Success("Trip update approved.");
    }

    private async Task<trip_registry_dto> ApproveOnlineTripAsync(transaction_data_archive approval, CancellationToken cancellationToken)
    {
        var trip = await _context.trip_registry.FirstOrDefaultAsync(x => x.tms_online_status == null && x.atc_no == approval.sales_doc_number, cancellationToken);
        if (trip is null)
        {
            return Failure(new trip_registry_dto(), "ATC was not found or already has an online trip.");
        }

        trip.tms_online_status = ApprovedStatus;
        trip.trip_id_sap = approval.trip_id;
        return Success("Online trip approved.");
    }

    private async Task<trip_registry_dto> ApproveOfflineRepushAsync(transaction_data_archive approval, CancellationToken cancellationToken)
    {
        var offlineTrip = await _context.transaction_data.FirstOrDefaultAsync(x => x.sync_status == false && x.gross != null && x.gross_time != null && x.sales_doc_number == approval.sales_doc_number && x.error_field == "OFFLINE", cancellationToken);
        if (offlineTrip is null)
        {
            return Failure(new trip_registry_dto(), "ATC was not found in offline posting errors.");
        }

        offlineTrip.error_field = "Repush";
        return Success("Offline repush approved.");
    }

    private async Task<trip_registry_dto> ApproveMolassesUpdateAsync(transaction_data_archive approval, CancellationToken cancellationToken)
    {
        var molassesDelivery = await _context.transaction_data.FirstOrDefaultAsync(x => x.sync_status == false && x.sales_doc_number == approval.sales_doc_number, cancellationToken);
        if (molassesDelivery is null)
        {
            return Failure(new trip_registry_dto(), "ATC was not found or is already synced.");
        }

        molassesDelivery.sales_order_item = molassesDelivery.sales_order_item == "FORTIFIED SUGAR" ? "MOLASSES" : "FORTIFIED SUGAR";
        return Success("Molasses update approved.");
    }

    private async Task<trip_registry_dto> ApproveCtesTripRemovalAsync(transaction_data_archive approval, CancellationToken cancellationToken)
    {
        var offlineTrip = await _context.transaction_data.FirstOrDefaultAsync(x => x.sync_status == false && x.gross != null && x.gross_time != null && x.sales_doc_number == approval.sales_doc_number && x.error_field == "Invalid Trip ID and Delivery combination", cancellationToken);
        if (offlineTrip is null)
        {
            return Failure(new trip_registry_dto(), "ATC was not found or has no invalid trip combination.");
        }

        offlineTrip.error_field = "Repush";
        offlineTrip.trip_id = "0000000000";
        return Success("CTES trip removal approved.");
    }

    private async Task<trip_registry_dto> ApproveManualDriverAddAsync(transaction_data_archive approval, CancellationToken cancellationToken)
    {
        var driverNo = approval.sales_doc_number;
        if (string.IsNullOrWhiteSpace(driverNo))
        {
            return Failure(new trip_registry_dto(), "Driver number is missing from the approval request.");
        }

        var exists = await _context.driver.AnyAsync(x => x.driver_no == driverNo, cancellationToken);
        if (exists)
        {
            return Failure(new trip_registry_dto(), "Driver number already exists.");
        }

        var driver = new driver
        {
            driver_no = driverNo,
            first_name = approval.driver,
            last_name = approval.transporter,
            plant = approval.loc,
            available = true,
            third_party = approval.code_a == "true",
            deletion_indicator = null,
            driver_status = "AV",
            dds_status = "ACTIVE"
        };

        _context.driver.Add(driver);
        return Success("Driver add request approved.");
    }

    private async Task<bool> CanApproveAsync(int userId, CancellationToken cancellationToken)
    {
        var user = await _context.user.FirstOrDefaultAsync(x => x.Id == userId && x.active == true && x.approved == true && x.status == "ACTIVE", cancellationToken);
        if (user is null)
        {
            return false;
        }

        if (user.can_approve == true)
        {
            return true;
        }

        var approvers = _configuration["Authentication:CheckerUsers"] ?? string.Empty;
        return approvers
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(approver => string.Equals(user.username, approver, StringComparison.OrdinalIgnoreCase));
    }

    private static ApprovalRequestDto MapApproval(transaction_data_archive approval)
    {
        return new ApprovalRequestDto
        {
            id = approval.id,
            requestType = approval.operator_weighin,
            requestTitle = GetTitle(approval.operator_weighin),
            atc = approval.operator_weighin == AtcSwapRequest ? approval.vehicle : approval.sales_doc_number,
            currentValue = approval.operator_weighin == AtcSwapRequest ? approval.vehicle : approval.sales_doc_number,
            requestedValue = GetRequestedValue(approval),
            tripId = approval.trip_id,
            reason = approval.seal ?? approval.message_field,
            requestedBy = approval.shp_point,
            requestedOn = approval.tare_time ?? approval.picking_time ?? approval.gross_time,
            status = approval.sales_doc_type
        };
    }

    private static string GetRequestedValue(transaction_data_archive approval)
    {
        return approval.operator_weighin switch
        {
            AtcSwapRequest => approval.sales_doc_number,
            UpdateDriverRequest => approval.driver,
            UpdateDestinationRequest => approval.destination,
            UpdateReceiverRequest => approval.sender,
            ChangeGradeRequest => approval.sales_order_item,
            AddOnlineTripRequest => approval.trip_id,
            AddDriverManuallyRequest => $"{approval.driver} {approval.transporter}".Trim(),
            _ => approval.seal ?? approval.message_field
        };
    }

    private static string GetTitle(string requestType)
    {
        return requestType switch
        {
            AtcSwapRequest => "ATC Swap",
            UpdateDriverRequest => "Driver Update",
            UpdateDestinationRequest => "Destination Update",
            UpdateReceiverRequest => "Receiver Update",
            ChangeGradeRequest => "ATC Grade Change",
            UpdateTripRequest => "Trip Validation",
            AddOnlineTripRequest => "Online Trip",
            RepushOfflineRequest => "Offline Repush",
            UpdateMolassesRequest => "Molasses Update",
            RemoveCtesTripRequest => "CTES Trip Removal",
            AddDriverManuallyRequest => "Manual Driver Add",
            _ => requestType
        };
    }

    private static string NormalizeActivity(string value)
    {
        return (value ?? "REQUEST").Replace(' ', '_').Replace('-', '_');
    }

    private static trip_registry_dto Success(string message)
    {
        return new trip_registry_dto { msg = message, status = true };
    }

    private static T Failure<T>(T response, string message) where T : trip_registry_dto
    {
        response.msg = message;
        response.status = false;
        return response;
    }
}
