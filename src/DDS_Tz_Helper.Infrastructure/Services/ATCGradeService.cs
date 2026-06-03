using DDS_Tz_Helper.Application.DTOs.ATCGrade;
using DDS_Tz_Helper.Application.DTOs.BusinessObjects;
using DDS_Tz_Helper.Application.Interfaces;
using DDS_Tz_Helper.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DDS_Tz_Helper.Infrastructure.Services;

public class ATCGradeService : IATCGradeService
{
    private const string GradeChangeActivity = "ATC_GRADE_CHANGE_HELPER";
    private readonly IDispatchDBContext _context;
    private readonly ITransactionLogService _transactionLogService;

    public ATCGradeService(IDispatchDBContext context, ITransactionLogService transactionLogService)
    {
        _context = context;
        _transactionLogService = transactionLogService;
    }

    public async Task<atc_datax> DoChangeATCGradeAsync(DoChangeATCGradeRequest request, CancellationToken cancellationToken = default)
    {
        var response = new atc_datax();

        try
        {
            if (request is null)
            {
                response.msg = "Invalid ATC grade change request";
                response.status = false;
                return response;
            }

            var atcDetails = await _context.atc_data
                .FirstOrDefaultAsync(x => x.sales_doc_number == request.atc && x.item_number != null, cancellationToken);

            var transDataDetails = await _context.transaction_data
                .FirstOrDefaultAsync(a => a.sales_doc_number == request.atc && a.gross_time != null, cancellationToken);

            if (transDataDetails != null)
            {
                response.msg = "ATC has been USED and WEIGHED-OUT!!!";
                response.status = false;
                return response;
            }

            if (atcDetails == null)
            {
                response.msg = "Wrong ATC Entered";
                response.status = false;
                return response;
            }

            atcDetails.sales_order_item = request.grade;
            await _transactionLogService.RecordLogAsync(request.atc, request.user_id, GradeChangeActivity, "ATC_GRADE_CHANGE : ", cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            response.msg = "ATC Updated Successfully";
            response.status = true;
            return response;
        }
        catch (Exception ex)
        {
            response.msg = "Error occured somewhere";
            response.status = false;
            response.excptn = ex.ToString();
            return response;
        }
    }
}
