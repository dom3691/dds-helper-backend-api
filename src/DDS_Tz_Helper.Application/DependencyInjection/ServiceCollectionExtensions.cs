using DDS_Tz_Helper.Application.Features.ATCGrade;
using DDS_Tz_Helper.Application.Features.ATCSwap;
using DDS_Tz_Helper.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DDS_Tz_Helper.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IATCSwapService, ATCSwapService>();
        services.AddScoped<IATCGradeService, ATCGradeService>();

        return services;
    }
}
