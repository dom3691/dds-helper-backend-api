using System.Reflection;
using DDS_Tz_Helper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDS_Tz_Helper.Infrastructure.Persistence.Configurations;

internal static class LegacyEntityConfigurationExtensions
{
    public static void ConfigureLegacyTable<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        string tableName,
        string keyName) where TEntity : class
    {
        builder.ToTable(tableName, tableBuilder => tableBuilder.UseSqlOutputClause(false));
        builder.HasKey(keyName);
        builder.IgnoreNavigationProperties();
        builder.ConfigureDecimalPrecision();
    }

    private static void IgnoreNavigationProperties<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class
    {
        foreach (var property in typeof(TEntity).GetProperties().Where(p => !IsScalarProperty(p.PropertyType)))
        {
            builder.Ignore(property.Name);
        }
    }

    private static void ConfigureDecimalPrecision<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class
    {
        foreach (var property in typeof(TEntity).GetProperties().Where(p => IsDecimalProperty(p.PropertyType)))
        {
            builder.Property(property.Name).HasPrecision(38, 6);
        }
    }

    private static bool IsScalarProperty(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;

        return type.GetTypeInfo().IsPrimitive
            || type.GetTypeInfo().IsEnum
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(DateTime)
            || type == typeof(DateTimeOffset)
            || type == typeof(TimeSpan)
            || type == typeof(Guid)
            || type == typeof(byte[]);
    }

    private static bool IsDecimalProperty(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        return type == typeof(decimal);
    }
}

public class AtcConfiguration : IEntityTypeConfiguration<atc>
{
    public void Configure(EntityTypeBuilder<atc> builder) => builder.ConfigureLegacyTable("atc", nameof(atc.Id));
}

public class AtcDataConfiguration : IEntityTypeConfiguration<atc_data>
{
    public void Configure(EntityTypeBuilder<atc_data> builder) => builder.ConfigureLegacyTable("atc_data", nameof(atc_data.Id));
}

public class AtcSessionConfiguration : IEntityTypeConfiguration<atc_session>
{
    public void Configure(EntityTypeBuilder<atc_session> builder) => builder.ConfigureLegacyTable("atc_session", nameof(atc_session.Id));
}

public class ChangeRequestConfiguration : IEntityTypeConfiguration<change_request>
{
    public void Configure(EntityTypeBuilder<change_request> builder) => builder.ConfigureLegacyTable("change_request", nameof(change_request.id));
}

public class DriverConfiguration : IEntityTypeConfiguration<driver>
{
    public void Configure(EntityTypeBuilder<driver> builder) => builder.ConfigureLegacyTable("driver", nameof(driver.Id));
}

public class GateAccessInfoConfiguration : IEntityTypeConfiguration<gateaccess_info>
{
    public void Configure(EntityTypeBuilder<gateaccess_info> builder) => builder.ConfigureLegacyTable("gateaccess_info", nameof(gateaccess_info.id));
}

public class StoDataConfiguration : IEntityTypeConfiguration<sto_data>
{
    public void Configure(EntityTypeBuilder<sto_data> builder) => builder.ConfigureLegacyTable("sto_data", nameof(sto_data.Id));
}

public class StoTransactionDataConfiguration : IEntityTypeConfiguration<sto_transaction_data>
{
    public void Configure(EntityTypeBuilder<sto_transaction_data> builder) => builder.ConfigureLegacyTable("sto_transaction_data", nameof(sto_transaction_data.id));
}

public class SystemSettingConfiguration : IEntityTypeConfiguration<system_setting>
{
    public void Configure(EntityTypeBuilder<system_setting> builder) => builder.ConfigureLegacyTable("system_setting", nameof(system_setting.Id));
}

public class TradeConfiguration : IEntityTypeConfiguration<Trade>
{
    public void Configure(EntityTypeBuilder<Trade> builder) => builder.ConfigureLegacyTable("Trade", nameof(Trade.id));
}

public class TransactionDataConfiguration : IEntityTypeConfiguration<transaction_data>
{
    public void Configure(EntityTypeBuilder<transaction_data> builder) => builder.ConfigureLegacyTable("transaction_data", nameof(transaction_data.id));
}

public class TransactionDataArchiveConfiguration : IEntityTypeConfiguration<transaction_data_archive>
{
    public void Configure(EntityTypeBuilder<transaction_data_archive> builder) => builder.ConfigureLegacyTable("transaction_data_archive", nameof(transaction_data_archive.id));
}

public class TransactionLogConfiguration : IEntityTypeConfiguration<transaction_log>
{
    public void Configure(EntityTypeBuilder<transaction_log> builder) => builder.ConfigureLegacyTable("transaction_log", nameof(transaction_log.Id));
}

public class TripRegistryConfiguration : IEntityTypeConfiguration<trip_registry>
{
    public void Configure(EntityTypeBuilder<trip_registry> builder) => builder.ConfigureLegacyTable("trip_registry", nameof(trip_registry.Id));
}

public class UserConfiguration : IEntityTypeConfiguration<user>
{
    public void Configure(EntityTypeBuilder<user> builder) => builder.ConfigureLegacyTable("user", nameof(user.Id));
}

public class WeightChangeConfiguration : IEntityTypeConfiguration<weight_change>
{
    public void Configure(EntityTypeBuilder<weight_change> builder) => builder.ConfigureLegacyTable("weight_change", nameof(weight_change.id));
}
