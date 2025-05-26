using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.Common;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssetsController: ControllerBase
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
        var pagedData = await _assetService.GetAssetsAsync(queryParameters);

        return new ApiResponse<PagedResult<AssetDto>>
        {
            Success = true,
            Message = "Successfully fetched a paginated list of assets",
            Data = pagedData
        };
    }
    
    [HttpGet("{id:Guid}")]
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
}