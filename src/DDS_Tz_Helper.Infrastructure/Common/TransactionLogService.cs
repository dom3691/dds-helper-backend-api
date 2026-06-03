using DDS_Tz_Helper.Application.Interfaces;
using DDS_Tz_Helper.Domain.Entities;
using DDS_Tz_Helper.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DDS_Tz_Helper.Infrastructure.Common;

public class TransactionLogService : ITransactionLogService
{
    private readonly IDispatchDBContext _context;

    public TransactionLogService(IDispatchDBContext context)
    {
        _context = context;
    }

    public async Task RecordLogAsync(string atc, int userId, string activity, string fieldName, CancellationToken cancellationToken = default)
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
