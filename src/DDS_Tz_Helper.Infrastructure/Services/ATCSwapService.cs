using DDS_Tz_Helper.Application.DTOs.ATCSwap;
using DDS_Tz_Helper.Application.DTOs.BusinessObjects;
using DDS_Tz_Helper.Application.Interfaces;
using DDS_Tz_Helper.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DDS_Tz_Helper.Infrastructure.Services;

public class ATCSwapService : IATCSwapService
{
    private const string BagsSalesType = "BAGS";
    private const string PendingStatus = "PENDING";
    private const string SwapRequestOperator = "ATC SWAP REQUEST";
    private const string SwapRequestActivity = "ATC_SWAP_REQUEST_HELPER";

    private readonly IDispatchDBContext _context;

    public ATCSwapService(IDispatchDBContext context)
    {
        _context = context;
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
        if (trxDetails is null)
        {
            response.status = false;
            response.msg = "Wrong ATC or ATC is already weighed-out";
            return response;
        }

        var archive = new transaction_data_archive
        {
            sales_doc_number = request.newATC,
            trip_id = request.trip_id,
            vehicle = request.atc,
            shp_point = request.user_id.ToString(),
            operator_weighin = SwapRequestOperator,
            sales_type = BagsSalesType,
            tare_time = DateTime.Now,
            sales_doc_type = PendingStatus,
            seal = request.reason
        };

        _context.transaction_data_archive.Add(archive);
        await RecordLogAsync(request.atc, request.user_id, SwapRequestActivity, $"{request.newATC} {request.reason}", cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        response.msg = "SWAP REQUEST SUBMITTED SUCCESSFULLY; APPROVAL IS PENDING!";
        response.status = true;
        return response;
    }

    private async Task RecordLogAsync(string atc, int userId, string activity, string fieldName, CancellationToken cancellationToken)
    {
        var currentSession = await _context.atc_session.OrderByDescending(a => a.session_start).FirstOrDefaultAsync(cancellationToken);

        var transactionLog = new transaction_log
        {
            atc = atc,
            activity = activity,
            field_name = fieldName,
            field_value = DateTime.Now.ToString(),
            trx_date = DateTime.Now,
            user_id = userId
        };

        if (currentSession?.status == "open")
        {
            transactionLog.session_id = currentSession.Id;
        }

        _context.transaction_log.Add(transactionLog);
    }
}
