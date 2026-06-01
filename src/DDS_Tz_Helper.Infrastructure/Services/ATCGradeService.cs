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

    public ATCGradeService(IDispatchDBContext context)
    {
        _context = context;
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
            await RecordLogAsync(request.atc, request.user_id, GradeChangeActivity, "ATC_GRADE_CHANGE : ", cancellationToken);
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

    private async Task RecordLogAsync(string atc, int userId, string activity, string fieldName, CancellationToken cancellationToken)
    {
        var currentSession = await _context.atc_session
            .OrderByDescending(a => a.session_start)
            .FirstOrDefaultAsync(cancellationToken);

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
