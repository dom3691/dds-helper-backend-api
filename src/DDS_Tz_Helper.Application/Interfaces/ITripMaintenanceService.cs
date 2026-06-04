using DDS_Tz_Helper.Application.DTOs.BusinessObjects;
using DDS_Tz_Helper.Application.DTOs.TripMaintenance;

namespace DDS_Tz_Helper.Application.Interfaces;

public interface ITripMaintenanceService
{
    Task<object> DoUpdateDriverAsync(DoUpdateDriverRequest request, CancellationToken cancellationToken = default);
    Task<object> DoUpdateDestinationAsync(DoUpdateDestinationRequest request, CancellationToken cancellationToken = default);
    Task<trip_registry_dto> DoUpdateTripAsync(DoUpdateTripRequest request, CancellationToken cancellationToken = default);
    Task<trip_registry_dto> AddOnlineTripAsync(AddOnlineTripRequest request, CancellationToken cancellationToken = default);
    Task<trip_registry_dto> AddDriverManuallyAsync(AddDriverManuallyRequest request, CancellationToken cancellationToken = default);
}
