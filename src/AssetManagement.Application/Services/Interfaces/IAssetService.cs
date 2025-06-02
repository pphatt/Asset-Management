using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.DTOs.Responses;
using AssetManagement.Contracts.Parameters;

namespace AssetManagement.Application.Services.Interfaces
{
    public interface IAssetService
    {
        Task<PagedResult<AssetDto>> GetAssetsAsync(string adminId, AssetQueryParameters queryParams);
        Task<AssetDetailsDto> GetAssetByIdAsync(Guid id);
        Task<CreateAssetResponseDto> CreateAssetAsync(CreateAssetRequestDto request, string adminId);
        Task<string> UpdateAssetAsync(string adminId, string assetCode, UpdateAssetRequestDto assetUpdateDto);
        
        Task<string> DeleteAssetAsync(Guid deletedBy, Guid id);
        
    }
}