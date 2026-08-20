namespace SearchJobs.Api;

public static class DispatchSearchServiceClientConfiguration
{
    public static IServiceCollection AddDispatchSearchServiceClientConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var baseUrl = configuration.GetSection("SearchService:BaseUrl").Get<string>()
            ?? throw new InvalidOperationException("Missing configuration value 'SearchService__BaseUrl'.");

        services.AddHttpClient<IDispatchSearchServiceClient, DispatchSearchServiceClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });

        return services;
    }
}
