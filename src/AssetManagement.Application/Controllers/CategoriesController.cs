using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Application.Extensions;
using AssetManagement.Contracts.Common;
using AssetManagement.Contracts.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.Application.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> Get()
        {
            var allCategories = await _categoryService.GetCategoriesAsync();
            return this.ToApiResponse(allCategories, "Successfully fetched all categories");
        }
    }
}