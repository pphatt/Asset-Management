namespace AssetManagement.Application.Extensions;

public static class SwaggerConfig
{
    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddRouting(options => options.LowercaseUrls = true);

        services.AddSwaggerGen();

        return services;
    }
}
