using DDS_Tz_Helper.Application.DTOs.ATCGrade;
using DDS_Tz_Helper.Application.DTOs.BusinessObjects;
using DDS_Tz_Helper.Application.Interfaces;
using DDS_Tz_Helper.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DDS_Tz_Helper.Infrastructure.Services;

public class ATCGradeService : IATCGradeService
{
    private const string GradeChangeActivity = "ATC_GRADE_CHANGE_HELPER";
    private static readonly HashSet<string> AllowedGrades = new(StringComparer.OrdinalIgnoreCase)
    {
        "42.5R,50KG+BAGGED+CEM2+OBA+LINE+1&2",
        "CEM II-32.5R -(U)-50KG Bagged Cement",
        "CEM II-42.5N -(U)-50KG Bagged Cement",
        "CEM II-32.5N -(U)-50KG Bagged Cement",
        "BAGBM_ZAM_ZM01"
    };

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

            var selectedGrade = request.grade?.Trim();
            if (string.IsNullOrWhiteSpace(request.atc) || string.IsNullOrWhiteSpace(selectedGrade))
            {
                response.msg = "ATC and grade are required.";
                response.status = false;
                return response;
            }

            if (!AllowedGrades.Contains(selectedGrade))
            {
                response.msg = "Invalid ATC grade selected. Please select one of the approved grades.";
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

            var pending = ApprovalService.CreatePendingRequest(ApprovalService.ChangeGradeRequest, request.atc, request.user_id);
            pending.sales_order_item = selectedGrade;

            _context.transaction_data_archive.Add(pending);
            await _transactionLogService.RecordLogAsync(request.atc, request.user_id, GradeChangeActivity, "ATC_GRADE_CHANGE : ", cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            response.msg = "ATC grade change request submitted successfully; approval is pending.";
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
