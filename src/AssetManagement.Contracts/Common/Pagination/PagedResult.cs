namespace AssetManagement.Contracts.Common.Pagination;

public class PagedResult<T>
{
    public PagedResult(IEnumerable<T> items, int totalCount, int pageSize, int pageNumber)
    {
        Items = items;
        PaginationMetadata = new PaginationMetadata
        {
            PageSize = pageSize,
            CurrentPage = pageNumber,
            TotalItems = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public IEnumerable<T> Items { get; set; }
    public PaginationMetadata PaginationMetadata { get; set; }
}