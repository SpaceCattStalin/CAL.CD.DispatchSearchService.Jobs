using Microsoft.Extensions.Options;

namespace SearchJobs.Api;

public static class DispatchSearchServiceClientConfiguration
{
    public static IServiceCollection AddDispatchSearchServiceClientConfiguration(this IServiceCollection services)
    {
        services.AddHttpClient<IDispatchSearchServiceClient, DispatchSearchServiceClient>((sp, client) =>
        {
            var baseUrl = sp.GetRequiredService<IOptions<AppSettings>>().Value.SearchService.BaseUrl;
            client.BaseAddress = new Uri(baseUrl);
        });

        return services;
    }
}
