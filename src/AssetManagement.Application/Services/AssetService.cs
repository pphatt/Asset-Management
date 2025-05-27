using AssetManagement.Application.Extensions;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.Parameters;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Extensions;
using AssetManagement.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Application.Services
{
    public class AssetService : IAssetService
    {
        private readonly IAssetRepository _assetRepository;

        public AssetService(IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
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
    }
}