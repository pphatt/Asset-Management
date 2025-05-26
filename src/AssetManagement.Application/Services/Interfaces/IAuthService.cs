using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.DTOs.Responses;

namespace AssetManagement.Application.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest);
    // Task RegisterAsync(RegisterRequestDto registerRequest);
    Task LogoutAsync();
    bool VerifyToken(string token);
    Task<ChangePasswordResponseDto> ChangePasswordAsync(string username, ChangePasswordRequestDto loginRequest);
}
