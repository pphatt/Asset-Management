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

        private async Task<Location> GetLocationByUserId(string userId)
        {
            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
            if (user is null)
            {
                throw new KeyNotFoundException($"User with id {userId} not found");
            }

            return user.Location;
        }

        public async Task<PagedResult<AssetDto>> GetAssetsAsync(string adminId, AssetQueryParameters queryParams)
        {
            var currentAdminLocation = await GetLocationByUserId(adminId);

            IQueryable<Asset> query = _assetRepository.GetAll()
                .AsNoTracking()
                .Include(a => a.Category)
                .ApplySearch(queryParams.SearchTerm)
                .ApplyFilters(queryParams.AssetStates, queryParams.AssetCategories, currentAdminLocation)
                .ApplySorting(queryParams.GetSortCriteria())
                .Where(c => c.IsDeleted != true);

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
                x => x.Id == id && x.IsDeleted != true,
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

            await _assetRepository.AddAsync(asset);
            await _assetRepository.SaveChangesAsync();

            return new CreateAssetResponseDto
            {
                Code = assetCode,
                Name = request.Name,
                CategoryName = category.Name,
                StateName = request.State.GetDisplayName()
            };
        }

        public async Task<string> UpdateAssetAsync(string adminId, string assetCode, UpdateAssetRequestDto request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Asset update data cannot be null");
            }

            var existingAsset = await _assetRepository.GetByCodeAsync(assetCode);
            if (existingAsset == null)
            {
                throw new KeyNotFoundException($"Cannot find asset with id {assetCode}");
            }

            AssetValidator.ValidateUpdateAsset(request);

            existingAsset.Name = request.Name ?? existingAsset.Name;
            if (request.State.HasValue)
            {
                existingAsset.State = MapAssetState(request.State.Value);
            }
            if (request.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetByIdAsync(request.CategoryId.Value);
                if (category == null)
                {
                    throw new KeyNotFoundException($"Cannot find category with id {request.CategoryId}");
                }
                existingAsset.CategoryId = request.CategoryId.Value;
            }
            existingAsset.Specification = request.Specification ?? existingAsset.Specification;
            if (!string.IsNullOrWhiteSpace(request.InstalledDate))
                existingAsset.InstalledDate = DateTime.Parse(request.InstalledDate);

            existingAsset.LastModifiedBy = Guid.Parse(adminId);
            existingAsset.LastModifiedDate = DateTime.UtcNow;

            _assetRepository.Update(existingAsset);
            await _assetRepository.SaveChangesAsync();

            return existingAsset.Code;
        }

        [assembly: InternalsVisibleTo("AssetManagement.Application.Tests")]
        private static AssetState MapAssetState(AssetStateDto dto)
        {
            return dto switch
            {
                AssetStateDto.Assigned => AssetState.Assigned,
                AssetStateDto.Available => AssetState.Available,
                AssetStateDto.NotAvailable => AssetState.NotAvailable,
                AssetStateDto.WaitingForRecycling => AssetState.WaitingForRecycling,
                AssetStateDto.Recycled => AssetState.Recycled,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        [assembly: InternalsVisibleTo("AssetManagement.Application.Tests")]
        private async Task<Location> GetCurrentAdminLocation(string userId)
        {
            var adminUser = await _userRepository.GetByIdAsync(Guid.Parse(userId));

            if (adminUser is null)
                throw new UnauthorizedAccessException("Cannot find admin to extract location");

            return adminUser.Location;
        }

        public async Task<string> DeleteAssetAsync(Guid deletedBy, Guid id)
        {
            var asset = await _assetRepository.GetByIdAsync(id);
            if (asset == null)
            {
                throw new KeyNotFoundException($"Cannot find asset with id {id}");
            }
            else if (asset.IsDeleted == true)
            {
                throw new InvalidOperationException($"Asset is already deleted");
            } 
            else
            {
                asset.DeletedBy = deletedBy;
                asset.DeletedDate = DateTime.UtcNow;
                asset.IsDeleted = true;
                
                _assetRepository.Update(asset);
                await _assetRepository.SaveChangesAsync();
                return asset.Code;  
            }
        }
    }
}