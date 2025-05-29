using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.DTOs.Responses;
using AssetManagement.Contracts.Parameters;

namespace AssetManagement.Application.Services.Interfaces
{
    public interface IAssetService
    {
        // <summary>
        // Retrieves a paginated list of users based on the provided query parameters
        // </summary>
        Task<PagedResult<AssetDto>> GetAssetsAsync(AssetQueryParameters queryParams);
        Task<AssetDetailsDto> GetAssetByIdAsync(Guid id);
        
        Task<CreateAssetResponseDto> CreateAssetAsync(CreateAssetRequestDto request, string adminId);
        
        
    }
}