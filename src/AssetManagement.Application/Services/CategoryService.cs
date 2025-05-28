using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            return await _categoryRepository.GetAll()
                .Select(c => new CategoryDto
                {
                    Name = c.Name,
                    Prefix = c.Prefix,
                }).ToListAsync();
        }
    }
}