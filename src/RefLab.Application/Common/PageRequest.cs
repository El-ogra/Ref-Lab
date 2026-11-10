namespace RefLab.Application.Common;

/// <summary>
/// A page request against a queryable set.
/// </summary>
public sealed record PageRequest(int PageNumber, int PageSize)
{
    /// <summary>The largest page size the application accepts.</summary>
    public const int MaximumPageSize = 200;

    /// <summary>The number of records to skip.</summary>
    public int Skip => (PageNumber - 1) * PageSize;

    /// <summary>The number of records to take.</summary>
    public int Take => PageSize;

    /// <summary>A request for the first page.</summary>
    public static PageRequest First(int pageSize = 25) => new(1, pageSize);
}
