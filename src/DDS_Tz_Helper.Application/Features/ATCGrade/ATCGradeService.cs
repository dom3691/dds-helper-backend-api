using DDS_Tz_Helper.Application.Common;
using DDS_Tz_Helper.Application.DTOs.ATCGrade;
using DDS_Tz_Helper.Application.DTOs.BusinessObjects;
using DDS_Tz_Helper.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DDS_Tz_Helper.Application.Features.ATCGrade;

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
        if (request is null)
        {
            return Failure("Invalid ATC grade change request");
        }

        var selectedGrade = request.grade?.Trim();
        if (string.IsNullOrWhiteSpace(request.atc) || string.IsNullOrWhiteSpace(selectedGrade))
        {
            return Failure("ATC and grade are required.");
        }

        if (!AllowedGrades.Contains(selectedGrade))
        {
            return Failure("Invalid ATC grade selected. Please select one of the approved grades.");
        }

        var atcDetails = await _context.atc_data
            .FirstOrDefaultAsync(x => x.sales_doc_number == request.atc && x.item_number != null, cancellationToken);

        var transDataDetails = await _context.transaction_data
            .FirstOrDefaultAsync(a => a.sales_doc_number == request.atc && a.gross_time != null, cancellationToken);

        if (transDataDetails is not null)
        {
            return Failure("ATC has been USED and WEIGHED-OUT!!!");
        }

        if (atcDetails is null)
        {
            return Failure("Wrong ATC Entered");
        }

        var pending = ApprovalRequestTypes.CreatePendingRequest(ApprovalRequestTypes.ChangeGradeRequest, request.atc, request.user_id);
        pending.sales_order_item = selectedGrade;

        _context.transaction_data_archive.Add(pending);
        await _transactionLogService.RecordLogAsync(request.atc, request.user_id, GradeChangeActivity, "ATC_GRADE_CHANGE : ", cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new atc_datax
        {
            msg = "ATC grade change request submitted successfully; approval is pending.",
            status = true
        };
    }

    private static atc_datax Failure(string message)
    {
        return new atc_datax
        {
            msg = message,
            status = false
        };
    }
}
