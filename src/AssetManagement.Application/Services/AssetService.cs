using System.Runtime.CompilerServices;
using AssetManagement.Application.Extensions;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Application.Validators;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.DTOs.Responses;
using AssetManagement.Contracts.Enums;
using AssetManagement.Contracts.Exceptions;
using AssetManagement.Contracts.Parameters;
using AssetManagement.Data.Helpers.Assets;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AssetManagement.Domain.Extensions;
using AssetManagement.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Application.Services
{
    public class AssetService : IAssetService
    {
        private readonly IAssetRepository _assetRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;

        public AssetService(IAssetRepository assetRepository, 
            ICategoryRepository categoryRepository, 
            IUserRepository userRepository)
        {
            _assetRepository = assetRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
        }

        public async Task<PagedResult<AssetDto>> GetAssetsAsync(AssetQueryParameters queryParams)
        {
            IQueryable<Asset> query = _assetRepository.GetAll()
                .AsNoTracking()
                .Include(x => x.Category)
                .ApplyAssetSearch(queryParams.SearchTerm)
                .ApplyAssetFilters(queryParams.AssetStates, queryParams.AssetCategories)
                .ApplyAssetSorting(queryParams.GetSortCriteria());

            int total = await query.CountAsync();

            var items = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .Select(u => new AssetDto
                {
                    Id = u.Id,
                    Code = u.Code,
                    Name = u.Name,
                    CategoryName = u.Category.Name,
                    State = u.State.GetDisplayName()
                })
                .ToListAsync();

            return new PagedResult<AssetDto>(items, total, queryParams.PageSize, queryParams.PageNumber);
        }

        public async Task<AssetDetailsDto> GetAssetByIdAsync(Guid id)
        {
            var asset = await _assetRepository.GetSingleAsync(
                x => x.Id == id,
                new CancellationToken(),
                false,
                x => x.Category);

            if (asset == null)
            {
                throw new KeyNotFoundException($"Cannot find asset with id {id}");
            }

            var result = new AssetDetailsDto(
                asset.Id,
                asset.Code,
                asset.Name,
                asset.State.GetDisplayName(),
                asset.Category.Name,
                asset.InstalledDate,
                asset.Location.GetDisplayName(),
                asset.Specification);

            return result;
        }

        public async Task<CreateAssetResponseDto> CreateAssetAsync(CreateAssetRequestDto request, string adminId)
        {
            
            AssetValidator.ValidateAsset(request); 
            // Validate category
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category is null)
            {
                throw new ApiExceptionTypes.NotFoundException("Category not found");
            }
            string assetCode = "";
            var lastAsset = _assetRepository.GetAll()
                .Where(c => c.Code.StartsWith(category.Prefix.ToUpper()))
                .OrderByDescending(c => c.Code).FirstOrDefault();

            assetCode = AssetGeneratorHelper.GenerateAssetCode(category.Prefix.ToUpper(), lastAsset);

            DateTimeOffset date = DateTimeOffset.Parse(request.InstalledDate!);
            var location = await GetCurrentAdminLocation(adminId);
            var asset = new Asset
            {
                Code = assetCode,
                CategoryId = request.CategoryId,
                Name = request.Name!,
                State = MapAssetState(request.State),
                InstalledDate = date,
                Location = location,
                Specification = request.Specifications!,
                CreatedBy = Guid.Parse(adminId),
                CreatedDate = DateTime.UtcNow,
                LastModifiedBy = Guid.Parse(adminId),
                LastModifiedDate = DateTime.UtcNow,
            };

            _assetRepository.Add(asset);
            return new CreateAssetResponseDto
            {
                Code = assetCode,
                Name = request.Name,
                CategoryName = category.Name,
                StateName = request.State.GetDisplayName()
            };
        }
        

        [assembly : InternalsVisibleTo("AssetManagement.Application.Tests")]
        private static AssetState MapAssetState(AssetStateDto dto)
        {
            return dto switch
            {
                AssetStateDto.Available => AssetState.Available,
                AssetStateDto.NotAvailable => AssetState.NotAvailable,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        [assembly : InternalsVisibleTo("AssetManagement.Application.Tests")]
        private async Task<Location> GetCurrentAdminLocation(string userId)
        {
            var adminUser = await _userRepository.GetByIdAsync(Guid.Parse(userId));

            if (adminUser is null)
                throw new UnauthorizedAccessException("Cannot find admin to extract location");

            return adminUser.Location;
        }

    }
}