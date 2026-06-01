using DDS_Tz_Helper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DDS_Tz_Helper.Application.Interfaces;

public interface IDispatchDBContext
{
    DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
    DbSet<atc_data> atc_data { get; set; }
    DbSet<atc_session> atc_session { get; set; }
    DbSet<transaction_data> transaction_data { get; set; }
    DbSet<transaction_data_archive> transaction_data_archive { get; set; }
    DbSet<transaction_log> transaction_log { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    EntityEntry Entry(object entity);
}
