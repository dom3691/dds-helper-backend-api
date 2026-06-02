using DDS_Tz_Helper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DDS_Tz_Helper.Application.Interfaces;

public interface IDispatchDBContext
{
    DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
    DbSet<atc> atcs { get; set; }
    DbSet<atc_data> atc_data { get; set; }
    DbSet<atc_session> atc_session { get; set; }
    DbSet<change_request> change_request { get; set; }
    DbSet<gateaccess_info> gateaccess_info { get; set; }
    DbSet<sto_data> sto_data { get; set; }
    DbSet<sto_transaction_data> sto_transaction_data { get; set; }
    DbSet<Trade> Trades { get; set; }
    DbSet<transaction_data> transaction_data { get; set; }
    DbSet<transaction_data_archive> transaction_data_archive { get; set; }
    DbSet<transaction_log> transaction_log { get; set; }
    DbSet<trip_registry> trip_registry { get; set; }
    DbSet<weight_change> weight_change { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    EntityEntry Entry(object entity);
}
