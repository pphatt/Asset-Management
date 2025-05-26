using Microsoft.AspNetCore.Mvc;
using AssetManagement.Application.Extensions;
using Microsoft.AspNetCore.Authorization;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.DTOs.Responses;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.Common;

namespace AssetManagement.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        var loginResponse = await _authService.LoginAsync(request);
        return this.ToApiResponse(loginResponse);
    }

    [HttpPost("password/change")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ChangePasswordResponseDto>>> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
        {
            return this.ToBadRequestApiResponse<ChangePasswordResponseDto>("User not authenticated");
        }
        var response = await _authService.ChangePasswordAsync(username, request);
        return this.ToApiResponse(response);
    }

    [HttpPost("token/verify")]
    public ActionResult<ApiResponse<bool>> VerifyToken()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return this.ToApiResponse(false, "Token is invalid");
        }
        return this.ToApiResponse(true, "Token is valid");
    }

    [HttpGet("test")]
    [Authorize(Roles = "Admin")]
    public ActionResult<ApiResponse<bool>> TestRoute()
    {
        return this.ToApiResponse(true);
    }
}
