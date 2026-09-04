namespace SearchJobs.Api.Models;

public class PageResponseWithCursor<T>(IEnumerable<T> Items, string? Cursor)
{
    public IEnumerable<T> Items { get; } = Items;
    public string? Cursor { get; } = Cursor;
}
