using DDS_Tz_Helper.Application.DTOs.BusinessObjects;
using DDS_Tz_Helper.Application.DTOs.Receiver;
using DDS_Tz_Helper.Application.Interfaces;
using DDS_Tz_Helper.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DDS_Tz_Helper.Infrastructure.Services;

public class ReceiverService : IReceiverService
{
    private const string UpdateReceiverActivity = "UPDATE_RECEIVER_HELPER";
    private readonly IDispatchDBContext _context;
    private readonly ITransactionLogService _transactionLogService;

    public ReceiverService(IDispatchDBContext context, ITransactionLogService transactionLogService)
    {
        _context = context;
        _transactionLogService = transactionLogService;
    }

    public async Task<atc_datax> DoUpdateReceiverAsync(DoUpdateReceiverRequest request, CancellationToken cancellationToken = default)
    {
        var response = new atc_datax();

        try
        {
            if (request is null)
            {
                response.msg = "Invalid receiver update request";
                response.status = false;
                return response;
            }

            var weighedOutTransaction = await _context.transaction_data
                .FirstOrDefaultAsync(a => a.sales_doc_number == request.atc && a.gross_time != null, cancellationToken);

            if (weighedOutTransaction != null)
            {
                response.msg = "ATC has been USED and WEIGHED-OUT!!!";
                response.status = false;
                return response;
            }

            var atcExists = await _context.atc_data
                .AnyAsync(x => x.sales_doc_number == request.atc, cancellationToken);

            if (!atcExists)
            {
                response.msg = "invalid ATC entered";
                response.status = false;
                return response;
            }

            var unusedAtcExists = await _context.atc_data
                .AnyAsync(x => x.sales_doc_number == request.atc && x.used == null, cancellationToken);

            if (!unusedAtcExists)
            {
                response.msg = "ATC is already used";
                response.status = false;
                return response;
            }

            var atcDetails = await _context.atc_data
                .FirstOrDefaultAsync(x => x.sales_doc_number == request.atc && x.customer_name == "" && x.used == null, cancellationToken);

            if (atcDetails is null)
            {
                response.msg = "Invalid ATC or Customer Already Exists!";
                response.status = false;
                return response;
            }

            atcDetails.customer_name = request.receiver;
            await _transactionLogService.RecordLogAsync(request.atc, request.user_id, UpdateReceiverActivity, "UPDATE_RECEIVER_HELPER: ", cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            response.msg = "Receiver/Customer Updated Successfully";
            response.status = true;
            return response;
        }
        catch
        {
            response.msg = "Error occured somewhere. cal on I.T";
            response.status = false;
            return response;
        }
    }
}
