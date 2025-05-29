using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Application.Validators;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;

        public CategoryService(ICategoryRepository categoryRepository, 
            IUserRepository userRepository)
        {
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequestDto request, string userId)
        {
            CategoryValidator.ValidateCreateCategory(request);
            await CategoryValidator.ValidateUserAsync(_userRepository, userId);
            await CategoryValidator.ValidateDuplicateNameAsync(_categoryRepository, request.Name);
            await CategoryValidator.ValidateDuplicatePrefixAsync(_categoryRepository, request.Prefix);

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Prefix = request.Prefix.ToUpper(),
                CreatedBy = Guid.Parse(userId),
                CreatedDate = DateTime.UtcNow,
            };
            _categoryRepository.Add(category);
            
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Prefix = category.Prefix,
            };
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            return await _categoryRepository.GetAll()
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Prefix = c.Prefix,
                }).ToListAsync();
        }
    }
}