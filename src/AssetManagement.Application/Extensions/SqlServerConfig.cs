using AssetManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Application.Extensions;

public static class SqlServerConfig
{
    public static IServiceCollection AddSqlServerConfig(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AssetManagementDbContext>(o =>
            o.UseSqlServer(configuration.GetConnectionString("Default")));

        return services;
    }
}