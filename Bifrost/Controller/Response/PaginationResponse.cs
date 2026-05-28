using Bifrost.Core.Domain;

namespace Bifrost.Response;

public class PaginationResponse<T, TR>(Pagination<T> pagination, Func<T, TR> mapper)
{
    public int Page { get; set; } = pagination.Page;
    public int Size { get; set; } = pagination.Size;
    public int Total { get; set; } = pagination.Total;
    public TR[] Results { get; set; } = pagination.Results.Select(mapper).ToArray();
}