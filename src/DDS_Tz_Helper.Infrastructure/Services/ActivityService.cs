using DDS_Tz_Helper.Application.DTOs.Activity;
using DDS_Tz_Helper.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DDS_Tz_Helper.Infrastructure.Services;

public class ActivityService : IActivityService
{
    private readonly IDispatchDBContext _context;

    public ActivityService(IDispatchDBContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<RecentActivityDto>> GetRecentActivitiesAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        var take = Math.Clamp(count, 1, 50);

        return await _context.transaction_log
            .OrderByDescending(x => x.trx_date)
            .ThenByDescending(x => x.Id)
            .Take(take)
            .GroupJoin(
                _context.user,
                log => log.user_id,
                user => user.Id,
                (log, users) => new { log, users })
            .SelectMany(
                item => item.users.DefaultIfEmpty(),
                (item, user) => new RecentActivityDto
                {
                    time = item.log.trx_date,
                    activity = item.log.activity,
                    reference = item.log.atc,
                    plant = string.Empty,
                    user = user != null ? user.username : item.log.user_id.ToString(),
                    status = "Logged"
                })
            .ToListAsync(cancellationToken);
    }
}
