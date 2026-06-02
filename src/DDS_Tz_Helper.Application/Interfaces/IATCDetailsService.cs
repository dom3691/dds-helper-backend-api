using DDS_Tz_Helper.Application.DTOs.ATCDetails;
using DDS_Tz_Helper.Application.DTOs.BusinessObjects;

namespace DDS_Tz_Helper.Application.Interfaces;

public interface IATCDetailsService
{
    Task<sto_datax> DoFetchATCDetailsAsync(FetchATCDetailsRequest request, CancellationToken cancellationToken = default);
    Task<Gate_Datax> DoFetchATCDetailsGateAsync(FetchATCDetailsRequest request, CancellationToken cancellationToken = default);
    Task<object> DoFetchATcForArchiveDetailsAsync(FetchATCDetailsRequest request, CancellationToken cancellationToken = default);
    Task<object> DoFetchATcForTareChangeDetailsAsync(FetchATCDetailsRequest request, CancellationToken cancellationToken = default);
    Task<sto_datax> DoFetchATCDetailsForQuantityUpdateAsync(FetchATCDetailsRequest request, CancellationToken cancellationToken = default);
    Task<sto_datax> DoFetchATcForGrossModifAsync(FetchATCDetailsRequest request, CancellationToken cancellationToken = default);
}
