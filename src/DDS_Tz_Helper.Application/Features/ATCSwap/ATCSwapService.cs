using DDS_Tz_Helper.Application.Common;
using DDS_Tz_Helper.Application.DTOs.ATCSwap;
using DDS_Tz_Helper.Application.DTOs.BusinessObjects;
using DDS_Tz_Helper.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DDS_Tz_Helper.Application.Features.ATCSwap;

public class ATCSwapService : IATCSwapService
{
    private const string BagsSalesType = "BAGS";
    private const string SwapRequestActivity = "ATC_SWAP_REQUEST_HELPER";

    private readonly IDispatchDBContext _context;
    private readonly ITransactionLogService _transactionLogService;

    public ATCSwapService(IDispatchDBContext context, ITransactionLogService transactionLogService)
    {
        _context = context;
        _transactionLogService = transactionLogService;
    }

    public async Task<Transaction_Datax> DoATCSwapAsync(DoATCSwapRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            return Failure("Invalid ATC swap request");
        }

        var atcDetails = await _context.atc_data
            .FirstOrDefaultAsync(x => x.sales_doc_number == request.newATC && x.used == null && x.sales_type == BagsSalesType, cancellationToken);

        if (atcDetails is null)
        {
            return Failure(request.TrxSwapPicking
                ? "Wrong ATC or ATC is already used"
                : "New ATC for the swap is either used or incorrect");
        }

        var archive = ApprovalRequestTypes.CreatePendingRequest(ApprovalRequestTypes.AtcSwapRequest, request.newATC, request.user_id);
        archive.trip_id = request.trip_id;
        archive.vehicle = request.atc;
        archive.sales_type = BagsSalesType;
        archive.seal = request.reason;

        _context.transaction_data_archive.Add(archive);
        await _transactionLogService.RecordLogAsync(request.atc, request.user_id, SwapRequestActivity, $"{request.newATC} {request.reason}", cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new Transaction_Datax
        {
            msg = "SWAP REQUEST SUBMITTED SUCCESSFULLY; APPROVAL IS PENDING!",
            status = true
        };
    }

    private static Transaction_Datax Failure(string message)
    {
        return new Transaction_Datax
        {
            msg = message,
            status = false
        };
    }
}
