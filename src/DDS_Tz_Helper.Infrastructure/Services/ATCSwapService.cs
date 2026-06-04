using DDS_Tz_Helper.Application.DTOs.ATCSwap;
using DDS_Tz_Helper.Application.DTOs.BusinessObjects;
using DDS_Tz_Helper.Application.Interfaces;
using DDS_Tz_Helper.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DDS_Tz_Helper.Infrastructure.Services;

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
        var response = new Transaction_Datax();

        if (request is null)
        {
            response.status = false;
            response.msg = "Invalid ATC swap request";
            return response;
        }

        var atcDetails = await _context.atc_data
            .FirstOrDefaultAsync(x => x.sales_doc_number == request.newATC && x.used == null && x.sales_type == BagsSalesType, cancellationToken);

        if (atcDetails is null)
        {
            response.status = false;
            response.msg = request.TrxSwapPicking
                ? "Wrong ATC or ATC is already used"
                : "New ATC for the swap is either used or incorrect";
            return response;
        }

        var transactionQuery = _context.transaction_data
            .Where(a => a.sales_doc_number == request.atc && a.gross == null);

        if (request.TrxSwapPicking)
        {
            transactionQuery = transactionQuery.Where(a => a.sales_type == BagsSalesType);
        }

        var trxDetails = await transactionQuery.FirstOrDefaultAsync(cancellationToken);
        //if (trxDetails is null)
        //{
        //    response.status = false;
        //    response.msg = "Wrong ATC or ATC is already weighed-out";
        //    return response;
        //}

        var archive = new transaction_data_archive
        {
            sales_doc_number = request.newATC,
            trip_id = request.trip_id,
            vehicle = request.atc,
            shp_point = request.user_id.ToString(),
            operator_weighin = ApprovalService.AtcSwapRequest,
            sales_type = BagsSalesType,
            tare_time = DateTime.Now,
            sales_doc_type = ApprovalService.PendingStatus,
            seal = request.reason
        };

        _context.transaction_data_archive.Add(archive);
        await _transactionLogService.RecordLogAsync(request.atc, request.user_id, SwapRequestActivity, $"{request.newATC} {request.reason}", cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        response.msg = "SWAP REQUEST SUBMITTED SUCCESSFULLY; APPROVAL IS PENDING!";
        response.status = true;
        return response;
    }
}
