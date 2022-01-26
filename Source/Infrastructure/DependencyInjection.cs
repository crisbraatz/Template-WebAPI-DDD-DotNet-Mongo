using Domain.Entities;
using Domain.Entities.Users;
using Infrastructure.Mongo.Common;
using Infrastructure.Mongo.Repositories.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddMongo();
    }

    private static void AddMongo(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionBuilder, ConnectionBuilder>();

        string connectionString = default;

        services.AddSingleton(provider =>
        {
            var builder = provider.GetService<IConnectionBuilder>();

            connectionString = builder?.GetConnectionString();

            return builder?.GetDatabase();
        });

        services.AddScoped<IBaseEntityRepository<User>, UserRepository>();

        services.AddHealthChecks()
            .AddMongoDb(connectionString);
    }
}