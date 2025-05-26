using AssetManagement.Data;
using AssetManagement.Data.Repositories;
using AssetManagement.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Application.Configurations;

public static class SqlServerConfiguration
{
    public static IServiceCollection AddSqlServerConfig(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AssetManagementDbContext>(o =>
            o.UseSqlServer(configuration.GetConnectionString("Default")));

        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}