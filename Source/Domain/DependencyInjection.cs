using Domain.Common.CustomDateTime;
using Microsoft.Extensions.DependencyInjection;

namespace Domain;

public static class DependencyInjection
{
    public static void AddDomain(this IServiceCollection services)
    {
        services.AddScoped<IDateTime, SystemDateTime>();
    }
}