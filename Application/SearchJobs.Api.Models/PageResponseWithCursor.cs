namespace SearchJobs.Api.Models;

public record class PageResponseWithCursor<T>(IEnumerable<T> Items, string? Cursor);
