using Microsoft.Extensions.Options;

namespace SearchJobs.Api;

public static class DispatchServiceClientConfiguration
{
    public static IServiceCollection AddDispatchServiceClientConfiguration(this IServiceCollection services)
    {
        services.AddHttpClient<IDispatchServiceClient, DispatchServiceClient>((sp, client) =>
        {
            var baseUrl = sp.GetRequiredService<IOptions<AppSettings>>().Value.DispatchService.BaseUrl;
            client.BaseAddress = new Uri(baseUrl);
        });

        return services;
    }
}
