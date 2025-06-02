using AssetManagement.Application.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace AssetManagement.Tests.Configurations;

public class SwaggerConfigurationTests
{
    private readonly ServiceCollection _services;

    public SwaggerConfigurationTests()
    {
        _services = new ServiceCollection();
    }

    [Fact]
    public void AddSwaggerConfig_ShouldReturnServiceCollection()
    {
        // Act
        var result = _services.AddSwaggerConfig();

        // Assert
        Assert.Same(_services, result);
    }

    [Fact]
    public void AddSwaggerConfig_ShouldRegisterSwaggerGen()
    {
        // Act
        _services.AddSwaggerConfig();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var swaggerGenOptions = serviceProvider.GetService<IOptions<SwaggerGenOptions>>();
        Assert.NotNull(swaggerGenOptions);
    }

    [Fact]
    public void AddSwaggerConfig_ShouldConfigureLowercaseUrls()
    {
        // Act
        _services.AddSwaggerConfig();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var routeOptions = serviceProvider.GetService<IOptions<Microsoft.AspNetCore.Routing.RouteOptions>>();
        Assert.NotNull(routeOptions);
        Assert.True(routeOptions.Value.LowercaseUrls);
    }

    

    [Fact]
    public void AddSwaggerConfig_ShouldRegisterRoutingServices()
    {
        // Act
        _services.AddSwaggerConfig();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var routeOptions = serviceProvider.GetService<IOptions<Microsoft.AspNetCore.Routing.RouteOptions>>();
        Assert.NotNull(routeOptions);
    }

    [Fact]
    public void AddSwaggerConfig_SwaggerGenOptions_ShouldBeConfigured()
    {
        // Act
        _services.AddSwaggerConfig();

        // Assert
        var descriptor = _services.FirstOrDefault(x => x.ServiceType == typeof(IConfigureOptions<SwaggerGenOptions>));
        Assert.NotNull(descriptor);
    }

    
}