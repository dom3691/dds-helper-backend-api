using DDS_Tz_Helper.Application.DTOs.ATCSwap;
using DDS_Tz_Helper.Application.DTOs.BusinessObjects;

namespace DDS_Tz_Helper.Application.Interfaces;

public interface IATCSwapService
{
    Task<Transaction_Datax> DoATCSwapAsync(DoATCSwapRequest request, CancellationToken cancellationToken = default);
}
