using DDS_Tz_Helper.Application.DTOs.BusinessObjects;
using DDS_Tz_Helper.Application.DTOs.PostingErrors;

namespace DDS_Tz_Helper.Application.Interfaces;

public interface IPostingErrorsService
{
    Task<trip_registry_dto> DoUpdateOfflineTripTForRepushAsync(AtcPostingErrorRequest request, CancellationToken cancellationToken = default);
    Task<trip_registry_dto> DoUpdateMolassesAsync(AtcPostingErrorRequest request, CancellationToken cancellationToken = default);
    Task<trip_registry_dto> DoRemoveTripIDForCTESAsync(AtcPostingErrorRequest request, CancellationToken cancellationToken = default);
}
