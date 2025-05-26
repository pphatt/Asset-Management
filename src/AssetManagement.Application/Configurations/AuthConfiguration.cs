using System.Text;
using AssetManagement.Application.Services;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Data.Helpers.Hashing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace AssetManagement.Application.Configurations;

public static class AuthConfiguration
{
    public static IServiceCollection AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
                };
            });

        services.AddAuthorization();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
