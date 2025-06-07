using AssetManagement.Application.Configurations;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Data.Helpers.Hashing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.Text;
using Xunit;

namespace AssetManagement.Tests.Configurations;

public class AuthConfigurationTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IConfigurationSection> _mockJwtSection;
    private readonly ServiceCollection _services;

    public AuthConfigurationTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockJwtSection = new Mock<IConfigurationSection>();
        _services = new ServiceCollection();

        // Setup JWT configuration values
        _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("test-issuer");
        _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("test-audience");
        _mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("this-is-a-very-long-secret-key-for-testing-purposes");
    }

    [Fact]
    public void AddAuthConfig_ShouldRegisterAuthenticationServices()
    {
        // Act
        var result = _services.AddAuthConfig(_mockConfiguration.Object);

        // Assert
        Assert.Same(_services, result);
        var serviceProvider = _services.BuildServiceProvider();
        
        // Verify authentication services are registered
        var authenticationService = serviceProvider.GetService<IAuthenticationService>();
        Assert.NotNull(authenticationService);
    }

    [Fact]
    public void AddAuthConfig_ShouldRegisterJwtBearerAuthentication()
    {
        // Act
        _services.AddAuthConfig(_mockConfiguration.Object);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var jwtBearerOptions = serviceProvider.GetService<IOptionsMonitor<JwtBearerOptions>>();
        Assert.NotNull(jwtBearerOptions);

        var options = jwtBearerOptions.Get(JwtBearerDefaults.AuthenticationScheme);
        Assert.NotNull(options.TokenValidationParameters);
        Assert.True(options.TokenValidationParameters.ValidateIssuer);
        Assert.True(options.TokenValidationParameters.ValidateAudience);
        Assert.True(options.TokenValidationParameters.ValidateLifetime);
        Assert.True(options.TokenValidationParameters.ValidateIssuerSigningKey);
    }

    [Fact]
    public void AddAuthConfig_ShouldConfigureTokenValidationParameters()
    {
        // Act
        _services.AddAuthConfig(_mockConfiguration.Object);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var jwtBearerOptions = serviceProvider.GetService<IOptionsMonitor<JwtBearerOptions>>();
        var options = jwtBearerOptions?.Get(JwtBearerDefaults.AuthenticationScheme);
        var tokenParams = options?.TokenValidationParameters;

        Assert.NotNull(tokenParams);
        Assert.Equal("test-issuer", tokenParams.ValidIssuer);
        Assert.Equal("test-audience", tokenParams.ValidAudience);
        Assert.IsType<SymmetricSecurityKey>(tokenParams.IssuerSigningKey);

        var symmetricKey = (SymmetricSecurityKey)tokenParams.IssuerSigningKey;
        var expectedKey = Encoding.UTF8.GetBytes("this-is-a-very-long-secret-key-for-testing-purposes");
        Assert.Equal(expectedKey, symmetricKey.Key);
    }

    [Fact]
    public void AddAuthConfig_ShouldRegisterPasswordHasher()
    {
        // Act
        _services.AddAuthConfig(_mockConfiguration.Object);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var passwordHasher = serviceProvider.GetService<IPasswordHasher>();
        Assert.NotNull(passwordHasher);
    }

    
}