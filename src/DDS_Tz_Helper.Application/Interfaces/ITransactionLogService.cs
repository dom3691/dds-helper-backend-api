namespace DDS_Tz_Helper.Application.Interfaces;

public interface ITransactionLogService
{
    Task RecordLogAsync(string atc, int userId, string activity, string fieldName, CancellationToken cancellationToken = default);
}
