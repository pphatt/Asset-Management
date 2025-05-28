using AssetManagement.Contracts.DTOs;

namespace AssetManagement.Application.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync();
    }
}