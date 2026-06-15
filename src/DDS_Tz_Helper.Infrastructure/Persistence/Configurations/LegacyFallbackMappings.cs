using DDS_Tz_Helper.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DDS_Tz_Helper.Infrastructure.Persistence;

internal static class LegacyFallbackMappings
{
    private static readonly IReadOnlyDictionary<Type, string> TableNames = new Dictionary<Type, string>
    {
        [typeof(C__MigrationHistory)] = "C__MigrationHistory",
        [typeof(C__RefactorLog)] = "C__RefactorLog",
        [typeof(alert_message)] = "alert_message",
        [typeof(alert_recipient)] = "alert_recipient",
        [typeof(approval)] = "approvals",
        [typeof(AspNetRole)] = "AspNetRoles",
        [typeof(AspNetUserClaim)] = "AspNetUserClaims",
        [typeof(AspNetUserLogin)] = "AspNetUserLogins",
        [typeof(AspNetUser)] = "AspNetUsers",
        [typeof(atc)] = "atc",
        [typeof(atc_card)] = "atc_card",
        [typeof(atc_correction)] = "atc_correction",
        [typeof(atc_data)] = "atc_data",
        [typeof(atc_session)] = "atc_session",
        [typeof(atc_transactions)] = "atc_transactions",
        [typeof(ATCDriverMaster)] = "ATCDriverMaster",
        [typeof(ATCDriverRemark)] = "ATCDriverRemark",
        [typeof(bag_counter)] = "bag_counter",
        [typeof(bag_counter_config)] = "bag_counter_config",
        [typeof(bin)] = "bin",
        [typeof(block_reason)] = "block_reason",
        [typeof(card)] = "card",
        [typeof(card_history)] = "card_history",
        [typeof(change_request)] = "change_request",
        [typeof(cng_master)] = "cng_master",
        [typeof(cng_posting)] = "cng_posting",
        [typeof(config_conversion)] = "config_conversion",
        [typeof(customer)] = "customer",
        [typeof(customer_block_reason)] = "customer_block_reason",
        [typeof(dds_config)] = "dds_config",
        [typeof(depot)] = "depot",
        [typeof(device)] = "device",
        [typeof(DocNumber)] = "DocNumbers",
        [typeof(DocType)] = "DocType",
        [typeof(driver)] = "driver",
        [typeof(eATC)] = "eATCs",
        [typeof(email)] = "emails",
        [typeof(equipment_detail)] = "equipment_detail",
        [typeof(equipment_master)] = "equipment_master",
        [typeof(estate_field)] = "estate_field",
        [typeof(excel_batch)] = "excel_batch",
        [typeof(excel_upload)] = "excel_upload",
        [typeof(fleet_master)] = "fleet_master",
        [typeof(fuel_master)] = "fuel_master",
        [typeof(fuel_posting)] = "fuel_posting",
        [typeof(gas_station)] = "gas_station",
        [typeof(gate_access_log)] = "gate_access_log",
        [typeof(gateaccess_info)] = "gateaccess_info",
        [typeof(harvest)] = "harvest",
        [typeof(harvest_crop_cycle)] = "harvest_crop_cycle",
        [typeof(harvest_estate)] = "harvest_estate",
        [typeof(harvest_field)] = "harvest_field",
        [typeof(harvest_plot)] = "harvest_plot",
        [typeof(harvest_spacing)] = "harvest_spacing",
        [typeof(harvest_variety)] = "harvest_variety",
        [typeof(Kna1)] = "Kna1",
        [typeof(loading_bay)] = "loading_bay",
        [typeof(loadingbay_queue)] = "loadingbay_queue",
        [typeof(master_file)] = "master_file",
        [typeof(material)] = "material",
        [typeof(median_tareweight)] = "median_tareweight",
        [typeof(module_access)] = "module_access",
        [typeof(monthly_target)] = "monthly_target",
        [typeof(outgrower)] = "outgrowers",
        [typeof(page_view)] = "page_view",
        [typeof(person)] = "people",
        [typeof(picking_correction)] = "picking_correction",
        [typeof(plant)] = "plant",
        [typeof(raw_weigher)] = "raw_weigher",
        [typeof(raw_weigher_trx)] = "raw_weigher_trx",
        [typeof(region_data)] = "region_data",
        [typeof(reservation)] = "reservation",
        [typeof(role)] = "roles",
        [typeof(ScaleWinNT)] = "ScaleWinNT",
        [typeof(shift)] = "shift",
        [typeof(sloc_material)] = "sloc_material",
        [typeof(sloc_shipping)] = "sloc_shipping",
        [typeof(sto_data)] = "sto_data",
        [typeof(sto_transaction_data)] = "sto_transaction_data",
        [typeof(sto_transaction_pgi)] = "sto_transaction_pgi",
        [typeof(sugar_trx)] = "sugar_trx",
        [typeof(system_setting)] = "system_setting",
        [typeof(tracker)] = "tracker",
        [typeof(Trade)] = "Trade",
        [typeof(transaction_data)] = "transaction_data",
        [typeof(transaction_log)] = "transaction_log",
        [typeof(transport_clearance)] = "transport_clearance",
        [typeof(transporter)] = "transporter",
        [typeof(trip_closure)] = "trip_closure",
        [typeof(trip_receipt_views)] = "trip_receipt_views",
        [typeof(trip_registry)] = "trip_registry",
        [typeof(trip_sync)] = "trip_sync",
        [typeof(truck)] = "truck",
        [typeof(user)] = "user",
        [typeof(user_modification)] = "user_modification",
        [typeof(user_request)] = "user_request",
        [typeof(vbbe)] = "vbbe",
        [typeof(vbpa)] = "vbpa",
        [typeof(vbpa1)] = "vbpas1",
        [typeof(waybill_views)] = "waybill_views",
        [typeof(weighbridge)] = "weighbridge",
        [typeof(weighbridge_images)] = "weighbridge_images",
        [typeof(weighbridge_images_sap)] = "weighbridge_images_sap",
        [typeof(weight_change)] = "weight_change",
        [typeof(weight_correction)] = "weight_correction",
        [typeof(weight_switch)] = "weight_switch",
        [typeof(atc_audit)] = "atc_audit",
        [typeof(trade_audit)] = "trade_audit",
        [typeof(UserList)] = "UserList",
        [typeof(Weighbridge_Line)] = "Weighbridge_Line",
        [typeof(view_atc_session_transaction)] = "view_atc_session_transaction",
        [typeof(view_bag_counter)] = "view_bag_counter",
        [typeof(view_dispatch)] = "view_dispatch",
        [typeof(view_driver_trip)] = "view_driver_trip",
        [typeof(view_equipment)] = "view_equipment",
        [typeof(view_equipment_full)] = "view_equipment_full",
        [typeof(view_equipment_trip)] = "view_equipment_trip",
        [typeof(view_estate_field)] = "view_estate_field",
        [typeof(view_field)] = "view_field",
        [typeof(view_harvest)] = "view_harvest",
        [typeof(view_outgrowers)] = "view_outgrowers",
        [typeof(view_plot)] = "view_plot",
        [typeof(view_sugar_trx)] = "view_sugar_trx",
        [typeof(view_transactions)] = "view_transactions",
        [typeof(atc_cleanup)] = "atc_cleanup",
        [typeof(close_transaction)] = "close_transaction",
        [typeof(destination)] = "destinations",
        [typeof(po_plant)] = "po_plant",
        [typeof(sales_doc_type)] = "sales_doc_type",
        [typeof(trx_data_audit)] = "trx_data_audit",
        [typeof(transaction_data_archive)] = "transaction_data_archive",
    };

    private static readonly HashSet<Type> CoreConfiguredTypes =
    [
        typeof(atc),
        typeof(atc_data),
        typeof(atc_session),
        typeof(change_request),
        typeof(driver),
        typeof(gateaccess_info),
        typeof(sto_data),
        typeof(sto_transaction_data),
        typeof(system_setting),
        typeof(Trade),
        typeof(transaction_data),
        typeof(transaction_data_archive),
        typeof(transaction_log),
        typeof(trip_registry),
        typeof(user),
        typeof(weight_change)
    ];

    public static void ConfigureLegacyFallbackMappings(this ModelBuilder modelBuilder)
    {
        foreach (var tableName in TableNames)
        {
            var clrType = tableName.Key;
            if (CoreConfiguredTypes.Contains(clrType))
            {
                continue;
            }

            var entityBuilder = modelBuilder.Entity(clrType).ToTable(tableName.Value, tableBuilder => tableBuilder.UseSqlOutputClause(false));
            foreach (var property in clrType.GetProperties().Where(p => !IsScalarProperty(p.PropertyType)))
            {
                entityBuilder.Ignore(property.Name);
            }

            foreach (var property in clrType.GetProperties().Where(p => IsDecimalProperty(p.PropertyType)))
            {
                entityBuilder.Property(property.Name).HasPrecision(38, 6);
            }

            var keyProperty = clrType.GetProperties()
                .Where(p => IsScalarProperty(p.PropertyType))
                .FirstOrDefault(p => p.Name == "Id" || p.Name == "id" || p.Name == clrType.Name + "Id" || p.Name == clrType.Name + "ID");

            if (keyProperty is null)
            {
                entityBuilder.HasNoKey();
            }
            else
            {
                entityBuilder.HasKey(keyProperty.Name);
            }
        }
    }

    private static bool IsScalarProperty(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;

        return type.IsPrimitive
            || type.IsEnum
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
