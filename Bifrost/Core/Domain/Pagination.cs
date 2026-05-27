namespace Bifrost.Core.Domain;

public class Pagination<T>(int page, int size, int total, T[] results)
{
    public int Page { get; set; } = page;
    public int Size { get; set; } = size;
    public int Total { get; set; } = total;
    public T[] Results { get; set; } = results;
}