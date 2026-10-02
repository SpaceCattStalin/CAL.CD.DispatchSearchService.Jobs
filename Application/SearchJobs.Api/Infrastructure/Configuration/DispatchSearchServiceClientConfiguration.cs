using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace SearchJobs.Api;

public static class DispatchSearchServiceClientConfiguration
{
    public static IServiceCollection AddDispatchSearchServiceClientConfiguration(this IServiceCollection services)
    {
        services.AddHttpClient<IDispatchSearchServiceClient, DispatchSearchServiceClient>((sp, client) =>
        {
            var settings = sp.GetRequiredService<IOptions<AppSettings>>().Value.SearchService;

            var baseUrl = sp.GetRequiredService<IOptions<AppSettings>>().Value.SearchService.BaseUrl;
            if (!string.IsNullOrEmpty(settings.ApiKey))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", settings.ApiKey);
            }
            client.BaseAddress = new Uri(baseUrl);
        });

        return services;
    }
}
