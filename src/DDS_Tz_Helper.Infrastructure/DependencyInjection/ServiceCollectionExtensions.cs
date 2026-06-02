using DDS_Tz_Helper.Application.Interfaces;
using DDS_Tz_Helper.Infrastructure.Persistence;
using DDS_Tz_Helper.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DDS_Tz_Helper.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DispatchDBContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DispatchDBEntities")));

        services.AddScoped<IDispatchDBContext>(provider => provider.GetRequiredService<DispatchDBContext>());
        services.AddScoped<IATCSwapService, ATCSwapService>();
        services.AddScoped<IATCGradeService, ATCGradeService>();
        services.AddScoped<IReceiverService, ReceiverService>();
        services.AddScoped<ITripMaintenanceService, TripMaintenanceService>();
        services.AddScoped<IPostingErrorsService, PostingErrorsService>();
        services.AddScoped<IATCDetailsService, ATCDetailsService>();

        return services;
    }
}




