# DDS_Tz_Helper Rebuild

.NET 8 clean architecture scaffold for the DDS_Tz_Helper solution.

## Projects

- `DDS_Tz_Helper.Domain`: EF entity POCOs migrated from the legacy EDMX-generated model.
- `DDS_Tz_Helper.Application`: DTOs and application interfaces.
- `DDS_Tz_Helper.Infrastructure`: EF Core SQL Server `DispatchDBContext` and dependency injection.
- `DDS_Tz_Helper.Api`: ASP.NET Core Web API host.

## Database

Update `src/DDS_Tz_Helper.Api/appsettings.json` with the SQL Server connection string you use in SSMS, then run EF Core migrations or scaffold from the existing database as needed.
