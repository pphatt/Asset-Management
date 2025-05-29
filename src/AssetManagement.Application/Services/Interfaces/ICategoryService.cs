using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Requests;

namespace AssetManagement.Application.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync();

        Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequestDto request, string userId);
    }
}