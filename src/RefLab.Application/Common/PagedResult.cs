namespace RefLab.Application.Common;

/// <summary>
/// One page of a query result together with the paging metadata.
/// </summary>
public sealed class PagedResult<T>
{
    /// <summary>Initialises a new page result.</summary>
    public PagedResult(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    /// <summary>The records on this page.</summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>The total number of matching records across every page.</summary>
    public int TotalCount { get; }

    /// <summary>The number of the current page, starting at one.</summary>
    public int PageNumber { get; }

    /// <summary>The size of the current page.</summary>
    public int PageSize { get; }

    /// <summary>The total number of pages.</summary>
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>Whether a previous page exists.</summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>Whether a next page exists.</summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>Projects a page of one element type into a page of another.</summary>
    public PagedResult<TResult> Map<TResult>(Func<T, TResult> selector) =>
        new(Items.Select(selector).ToArray(), TotalCount, PageNumber, PageSize);
}
