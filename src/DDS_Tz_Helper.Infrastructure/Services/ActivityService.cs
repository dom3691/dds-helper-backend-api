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

        var logs = await _context.transaction_log
            .OrderByDescending(x => x.trx_date)
            .ThenByDescending(x => x.Id)
            .Take(take)
            .ToListAsync(cancellationToken);

        var userIds = logs
            .Where(x => x.user_id.HasValue)
            .Select(x => x.user_id!.Value)
            .Distinct()
            .ToList();

        var users = await _context.user
            .Where(x => userIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.username, cancellationToken);

        return logs.Select(log => new RecentActivityDto
        {
            time = log.trx_date,
            activity = log.activity,
            reference = log.atc,
            plant = string.Empty,
            user = log.user_id.HasValue && users.TryGetValue(log.user_id.Value, out var username) ? username : log.user_id?.ToString(),
            status = "Logged"
        }).ToList();
    }
}
