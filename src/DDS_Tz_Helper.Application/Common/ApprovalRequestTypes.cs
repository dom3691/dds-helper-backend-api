using DDS_Tz_Helper.Domain.Entities;

namespace DDS_Tz_Helper.Application.Common;

public static class ApprovalRequestTypes
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

    public static readonly string[] SupportedRequestTypes =
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
}
