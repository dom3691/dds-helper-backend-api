using DDS_Tz_Helper.Application.DTOs.Activity;

namespace DDS_Tz_Helper.Application.Interfaces;

public interface IActivityService
{
    Task<IReadOnlyList<RecentActivityDto>> GetRecentActivitiesAsync(int count = 10, CancellationToken cancellationToken = default);
}
