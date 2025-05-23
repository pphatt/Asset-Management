using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.DTOs.Responses;

namespace AssetManagement.Application.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
    // Task RegisterAsync(RegisterRequestDto registerRequest);
    Task LogoutAsync();
    bool VerifyToken(string token);
    Task<ChangePasswordResponse> ChangePasswordAsync(string username, ChangePasswordRequest loginRequest);
}
