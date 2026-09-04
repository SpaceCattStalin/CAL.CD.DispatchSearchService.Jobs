using SearchJobs.Api.Models;

namespace SearchJobs.Api;

public interface IDispatchServiceClient
{
    Task<PageResponseWithCursor<DispatchWriterDto>> GetAsync(string cursor, string auth, int limit = 500);

}
