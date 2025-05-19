using Microsoft.AspNetCore.Mvc;
using AssetManagement.Application.Extensions;
using Microsoft.AspNetCore.Authorization;
using AssetManagement.Application.Services.Interfaces;
using System.Text.Json;
using AssetManagement.Contracts.DTOs.Resquest;
using AssetManagement.Contracts.DTOs.Response;
using AssetManagement.Contracts.DTOs;

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
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        var loginResponse = await _authService.LoginAsync(request);
        Response.Cookies.Append("access_token", loginResponse.AccessToken);
        Response.Cookies.Append("profile", JsonSerializer.Serialize(loginResponse.UserInfo));
        return this.ToApiResponse(loginResponse);
    }

    [HttpPost("change-password")]
    public async Task<ActionResult<ApiResponse<ChangePasswordResponse>>> ChangePassword([FromQuery] string username, [FromBody] ChangePasswordRequest request)
    {
        var response = await _authService.ChangePasswordAsync(username, request);
        return this.ToApiResponse(response);

    }
    
    [HttpPost("token")]
    public async Task<ActionResult<ApiResponse<bool>>> VerifyToken([FromBody] string token)
    {
        var response = await _authService.VerifyToken(token);
        return this.ToApiResponse(response);
    }

    [HttpGet("test")]
    [Authorize(Roles = "Admin")]
    public ActionResult<ApiResponse<bool>> TestRoute()
    {
        return this.ToApiResponse(true);
    }
}
