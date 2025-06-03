using System.Security.Claims;
using AssetManagement.Application.Extensions;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.Common;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.DTOs.Responses;
using AssetManagement.Contracts.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssetsController : ControllerBase
{
    private readonly IAssetService _assetService;

    public AssetsController(IAssetService assetService)
    {
        _assetService = assetService;
    }

    [HttpGet("")]
    [Authorize(Roles = "Admin, Staff")]
    public async Task<ActionResult<ApiResponse<PagedResult<AssetDto>>>> Get([FromQuery] AssetQueryParameters queryParameters)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
        {
            throw new UnauthorizedAccessException("Cannot retrieve user id from claims");
        }

        var pagedData = await _assetService.GetAssetsAsync(userId, queryParameters);

        return new ApiResponse<PagedResult<AssetDto>>
        {
            Success = true,
            Message = "Successfully fetched a paginated list of assets",
            Data = pagedData
        };
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin, Staff")]
    public async Task<ApiResponse<AssetDetailsDto>> GetById(Guid id)
    {
        var result = await _assetService.GetAssetByIdAsync(id);
        return new ApiResponse<AssetDetailsDto>
        {
            Success = true,
            Message = "Successfully fetched an asset details",
            Data = result,
        };
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<CreateAssetResponseDto>>> Create([FromBody] CreateAssetRequestDto dto)
    {
        var adminID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (adminID is null)
            throw new UnauthorizedAccessException("Cannot retrieve user id from claims");
        var createAsset = await _assetService.CreateAssetAsync(dto, adminID);
        return this.ToCreatedApiResponse(createAsset, "Successfully created a new asset");
    }

    [HttpPatch("{assetCode}")]
    [Authorize(Roles = "Admin")]
    public async Task<ApiResponse<string>> Update(string assetCode, [FromBody] UpdateAssetRequestDto request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
            throw new UnauthorizedAccessException("Cannot retrieve user id from claims");

        var updatedAssetCode = await _assetService.UpdateAssetAsync(userId, assetCode, request);

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Successfully updated asset.",
            Data = updatedAssetCode,
        };
    }

    [HttpDelete("{assetId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(Guid assetId)
    {
        var deletedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("Cannot retrieve user id from claims");
        var deletedAsset = await _assetService.DeleteAssetAsync(Guid.Parse(deletedBy!), assetId);
        return new ApiResponse<string>
        {
            Success = true,
            Message = "Successfully deleted asset.",
            Data = deletedAsset,
        };
    }
    
}