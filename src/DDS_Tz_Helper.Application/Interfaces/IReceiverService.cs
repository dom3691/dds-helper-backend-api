using DDS_Tz_Helper.Application.DTOs.BusinessObjects;
using DDS_Tz_Helper.Application.DTOs.Receiver;

namespace DDS_Tz_Helper.Application.Interfaces;

public interface IReceiverService
{
    Task<atc_datax> DoUpdateReceiverAsync(DoUpdateReceiverRequest request, CancellationToken cancellationToken = default);
}
