using AssetManagement.Contracts.DTOs.Response;
using AssetManagement.Contracts.DTOs.Resquest;

namespace AssetManagement.Application.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
    // Task RegisterAsync(RegisterRequestDto registerRequest);
    Task LogoutAsync();
    Task<bool> VerifyToken(string token);
    Task<ChangePasswordResponse> ChangePasswordAsync(string username, ChangePasswordRequest loginRequest);
}
