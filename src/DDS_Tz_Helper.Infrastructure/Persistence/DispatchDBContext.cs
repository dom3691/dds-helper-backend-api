using DDS_Tz_Helper.Application.Interfaces;
using DDS_Tz_Helper.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DDS_Tz_Helper.Infrastructure.Persistence;

public partial class DispatchDBContext : DbContext, IDispatchDBContext
{
    public DispatchDBContext(DbContextOptions<DispatchDBContext> options) : base(options)
    {
    }

    public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
    public virtual DbSet<C__RefactorLog> C__RefactorLog { get; set; }
    public virtual DbSet<alert_message> alert_message { get; set; }
    public virtual DbSet<alert_recipient> alert_recipient { get; set; }
    public virtual DbSet<approval> approvals { get; set; }
    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
    public virtual DbSet<atc> atcs { get; set; }
    public virtual DbSet<atc_card> atc_card { get; set; }
    public virtual DbSet<atc_correction> atc_correction { get; set; }
    public virtual DbSet<atc_data> atc_data { get; set; }
    public virtual DbSet<atc_session> atc_session { get; set; }
    public virtual DbSet<atc_transactions> atc_transactions { get; set; }
    public virtual DbSet<ATCDriverMaster> ATCDriverMasters { get; set; }
    public virtual DbSet<ATCDriverRemark> ATCDriverRemarks { get; set; }
    public virtual DbSet<bag_counter> bag_counter { get; set; }
    public virtual DbSet<bag_counter_config> bag_counter_config { get; set; }
    public virtual DbSet<bin> bins { get; set; }
    public virtual DbSet<block_reason> block_reason { get; set; }
    public virtual DbSet<card> cards { get; set; }
    public virtual DbSet<card_history> card_history { get; set; }
    public virtual DbSet<change_request> change_request { get; set; }
    public virtual DbSet<cng_master> cng_master { get; set; }
    public virtual DbSet<cng_posting> cng_posting { get; set; }
    public virtual DbSet<config_conversion> config_conversion { get; set; }
    public virtual DbSet<customer> customers { get; set; }
    public virtual DbSet<customer_block_reason> customer_block_reason { get; set; }
    public virtual DbSet<dds_config> dds_config { get; set; }
    public virtual DbSet<depot> depots { get; set; }
    public virtual DbSet<device> devices { get; set; }
    public virtual DbSet<DocNumber> DocNumbers { get; set; }
    public virtual DbSet<DocType> DocTypes { get; set; }
    public virtual DbSet<driver> driver { get; set; }
    public virtual DbSet<eATC> eATCs { get; set; }
    public virtual DbSet<email> emails { get; set; }
    public virtual DbSet<equipment_detail> equipment_detail { get; set; }
    public virtual DbSet<equipment_master> equipment_master { get; set; }
    public virtual DbSet<estate_field> estate_field { get; set; }
    public virtual DbSet<excel_batch> excel_batch { get; set; }
    public virtual DbSet<excel_upload> excel_upload { get; set; }
    public virtual DbSet<fleet_master> fleet_master { get; set; }
    public virtual DbSet<fuel_master> fuel_master { get; set; }
    public virtual DbSet<fuel_posting> fuel_posting { get; set; }
    public virtual DbSet<gas_station> gas_station { get; set; }
    public virtual DbSet<gate_access_log> gate_access_log { get; set; }
    public virtual DbSet<gateaccess_info> gateaccess_info { get; set; }
    public virtual DbSet<harvest> harvests { get; set; }
    public virtual DbSet<harvest_crop_cycle> harvest_crop_cycle { get; set; }
    public virtual DbSet<harvest_estate> harvest_estate { get; set; }
    public virtual DbSet<harvest_field> harvest_field { get; set; }
    public virtual DbSet<harvest_plot> harvest_plot { get; set; }
    public virtual DbSet<harvest_spacing> harvest_spacing { get; set; }
    public virtual DbSet<harvest_variety> harvest_variety { get; set; }
    public virtual DbSet<Kna1> Kna1 { get; set; }
    public virtual DbSet<loading_bay> loading_bay { get; set; }
    public virtual DbSet<loadingbay_queue> loadingbay_queue { get; set; }
    public virtual DbSet<master_file> master_file { get; set; }
    public virtual DbSet<material> materials { get; set; }
    public virtual DbSet<median_tareweight> median_tareweight { get; set; }
    public virtual DbSet<module_access> module_access { get; set; }
    public virtual DbSet<monthly_target> monthly_target { get; set; }
    public virtual DbSet<outgrower> outgrowers { get; set; }
    public virtual DbSet<page_view> page_view { get; set; }
    public virtual DbSet<person> people { get; set; }
    public virtual DbSet<picking_correction> picking_correction { get; set; }
    public virtual DbSet<plant> plants { get; set; }
    public virtual DbSet<raw_weigher> raw_weigher { get; set; }
    public virtual DbSet<raw_weigher_trx> raw_weigher_trx { get; set; }
    public virtual DbSet<region_data> region_data { get; set; }
    public virtual DbSet<reservation> reservations { get; set; }
    public virtual DbSet<role> roles { get; set; }
    public virtual DbSet<ScaleWinNT> ScaleWinNTs { get; set; }
    public virtual DbSet<shift> shifts { get; set; }
    public virtual DbSet<sloc_material> sloc_material { get; set; }
    public virtual DbSet<sloc_shipping> sloc_shipping { get; set; }
    public virtual DbSet<sto_data> sto_data { get; set; }
    public virtual DbSet<sto_transaction_data> sto_transaction_data { get; set; }
    public virtual DbSet<sto_transaction_pgi> sto_transaction_pgi { get; set; }
    public virtual DbSet<sugar_trx> sugar_trx { get; set; }
    public virtual DbSet<system_setting> system_setting { get; set; }
    public virtual DbSet<tracker> trackers { get; set; }
    public virtual DbSet<Trade> Trade { get; set; }
    public virtual DbSet<transaction_data> transaction_data { get; set; }
    public virtual DbSet<transaction_log> transaction_log { get; set; }
    public virtual DbSet<transport_clearance> transport_clearance { get; set; }
    public virtual DbSet<transporter> transporters { get; set; }
    public virtual DbSet<trip_closure> trip_closure { get; set; }
    public virtual DbSet<trip_receipt_views> trip_receipt_views { get; set; }
    public virtual DbSet<trip_registry> trip_registry { get; set; }
    public virtual DbSet<trip_sync> trip_sync { get; set; }
    public virtual DbSet<truck> trucks { get; set; }
    public virtual DbSet<user> user { get; set; }
    public virtual DbSet<user_modification> user_modification { get; set; }
    public virtual DbSet<user_request> user_request { get; set; }
    public virtual DbSet<vbbe> vbbes { get; set; }
    public virtual DbSet<vbpa> vbpas { get; set; }
    public virtual DbSet<vbpa1> vbpas1 { get; set; }
    public virtual DbSet<waybill_views> waybill_views { get; set; }
    public virtual DbSet<weighbridge> weighbridges { get; set; }
    public virtual DbSet<weighbridge_images> weighbridge_images { get; set; }
    public virtual DbSet<weighbridge_images_sap> weighbridge_images_sap { get; set; }
    public virtual DbSet<weight_change> weight_change { get; set; }
    public virtual DbSet<weight_correction> weight_correction { get; set; }
    public virtual DbSet<weight_switch> weight_switch { get; set; }
    public virtual DbSet<atc_audit> atc_audit { get; set; }
    public virtual DbSet<trade_audit> trade_audit { get; set; }
    public virtual DbSet<UserList> UserLists { get; set; }
    public virtual DbSet<Weighbridge_Line> Weighbridge_Line { get; set; }
    public virtual DbSet<view_atc_session_transaction> view_atc_session_transaction { get; set; }
    public virtual DbSet<view_bag_counter> view_bag_counter { get; set; }
    public virtual DbSet<view_dispatch> view_dispatch { get; set; }
    public virtual DbSet<view_driver_trip> view_driver_trip { get; set; }
    public virtual DbSet<view_equipment> view_equipment { get; set; }
    public virtual DbSet<view_equipment_full> view_equipment_full { get; set; }
    public virtual DbSet<view_equipment_trip> view_equipment_trip { get; set; }
    public virtual DbSet<view_estate_field> view_estate_field { get; set; }
    public virtual DbSet<view_field> view_field { get; set; }
    public virtual DbSet<view_harvest> view_harvest { get; set; }
    public virtual DbSet<view_outgrowers> view_outgrowers { get; set; }
    public virtual DbSet<view_plot> view_plot { get; set; }
    public virtual DbSet<view_sugar_trx> view_sugar_trx { get; set; }
    public virtual DbSet<view_transactions> view_transactions { get; set; }
    public virtual DbSet<atc_cleanup> atc_cleanup { get; set; }
    public virtual DbSet<close_transaction> close_transaction { get; set; }
    public virtual DbSet<destination> destinations { get; set; }
    public virtual DbSet<po_plant> po_plant { get; set; }
    public virtual DbSet<sales_doc_type> sales_doc_type { get; set; }
    public virtual DbSet<trx_data_audit> trx_data_audit { get; set; }
    public virtual DbSet<transaction_data_archive> transaction_data_archive { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var tableNames = new Dictionary<Type, string>
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
            [typeof(atc)] = "atcs",
            [typeof(atc_card)] = "atc_card",
            [typeof(atc_correction)] = "atc_correction",
            [typeof(atc_data)] = "atc_data",
            [typeof(atc_session)] = "atc_session",
            [typeof(atc_transactions)] = "atc_transactions",
            [typeof(ATCDriverMaster)] = "ATCDriverMasters",
            [typeof(ATCDriverRemark)] = "ATCDriverRemarks",
            [typeof(bag_counter)] = "bag_counter",
            [typeof(bag_counter_config)] = "bag_counter_config",
            [typeof(bin)] = "bins",
            [typeof(block_reason)] = "block_reason",
            [typeof(card)] = "cards",
            [typeof(card_history)] = "card_history",
            [typeof(change_request)] = "change_request",
            [typeof(cng_master)] = "cng_master",
            [typeof(cng_posting)] = "cng_posting",
            [typeof(config_conversion)] = "config_conversion",
            [typeof(customer)] = "customers",
            [typeof(customer_block_reason)] = "customer_block_reason",
            [typeof(dds_config)] = "dds_config",
            [typeof(depot)] = "depots",
            [typeof(device)] = "devices",
            [typeof(DocNumber)] = "DocNumbers",
            [typeof(DocType)] = "DocTypes",
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
            [typeof(harvest)] = "harvests",
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
            [typeof(material)] = "materials",
            [typeof(median_tareweight)] = "median_tareweight",
            [typeof(module_access)] = "module_access",
            [typeof(monthly_target)] = "monthly_target",
            [typeof(outgrower)] = "outgrowers",
            [typeof(page_view)] = "page_view",
            [typeof(person)] = "people",
            [typeof(picking_correction)] = "picking_correction",
            [typeof(plant)] = "plants",
            [typeof(raw_weigher)] = "raw_weigher",
            [typeof(raw_weigher_trx)] = "raw_weigher_trx",
            [typeof(region_data)] = "region_data",
            [typeof(reservation)] = "reservations",
            [typeof(role)] = "roles",
            [typeof(ScaleWinNT)] = "ScaleWinNTs",
            [typeof(shift)] = "shifts",
            [typeof(sloc_material)] = "sloc_material",
            [typeof(sloc_shipping)] = "sloc_shipping",
            [typeof(sto_data)] = "sto_data",
            [typeof(sto_transaction_data)] = "sto_transaction_data",
            [typeof(sto_transaction_pgi)] = "sto_transaction_pgi",
            [typeof(sugar_trx)] = "sugar_trx",
            [typeof(system_setting)] = "system_setting",
            [typeof(tracker)] = "trackers",
            [typeof(Trade)] = "Trade",
            [typeof(transaction_data)] = "transaction_data",
            [typeof(transaction_log)] = "transaction_log",
            [typeof(transport_clearance)] = "transport_clearance",
            [typeof(transporter)] = "transporters",
            [typeof(trip_closure)] = "trip_closure",
            [typeof(trip_receipt_views)] = "trip_receipt_views",
            [typeof(trip_registry)] = "trip_registry",
            [typeof(trip_sync)] = "trip_sync",
            [typeof(truck)] = "trucks",
            [typeof(user)] = "user",
            [typeof(user_modification)] = "user_modification",
            [typeof(user_request)] = "user_request",
            [typeof(vbbe)] = "vbbes",
            [typeof(vbpa)] = "vbpas",
            [typeof(vbpa1)] = "vbpas1",
            [typeof(waybill_views)] = "waybill_views",
            [typeof(weighbridge)] = "weighbridges",
            [typeof(weighbridge_images)] = "weighbridge_images",
            [typeof(weighbridge_images_sap)] = "weighbridge_images_sap",
            [typeof(weight_change)] = "weight_change",
            [typeof(weight_correction)] = "weight_correction",
            [typeof(weight_switch)] = "weight_switch",
            [typeof(atc_audit)] = "atc_audit",
            [typeof(trade_audit)] = "trade_audit",
            [typeof(UserList)] = "UserLists",
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

        foreach (var tableName in tableNames)
        {
            var clrType = tableName.Key;
            var entityBuilder = modelBuilder.Entity(clrType).ToTable(tableName.Value, tableBuilder => tableBuilder.UseSqlOutputClause(false));
            foreach (var property in clrType.GetProperties().Where(p => !IsScalarProperty(p.PropertyType)))
            {
                entityBuilder.Ignore(property.Name);
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
}

