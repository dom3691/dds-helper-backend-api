//using DDS_Tz_Helper.Domain.Entities;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DDS_Tz_Helper.Application.Common.Helpers
//{
//    public class TransactionLogging
//    {
//        private async Task RecordLogAsync(string atc, int userId, string activity, string fieldName, CancellationToken cancellationToken)
//        {
//            var currentSession = await _context.atc_session
//                .OrderByDescending(a => a.session_start)
//                .FirstOrDefaultAsync(cancellationToken);

//            var transactionLog = new transaction_log
//            {
//                atc = atc,
//                activity = activity,
//                field_name = fieldName,
//                field_value = DateTime.Now.ToString(),
//                trx_date = DateTime.Now,
//                user_id = userId
//            };

//            if (currentSession?.status == "open")
//            {
//                transactionLog.session_id = currentSession.Id;
//            }

//            _context.transaction_log.Add(transactionLog);
//        }
//    }
//}
