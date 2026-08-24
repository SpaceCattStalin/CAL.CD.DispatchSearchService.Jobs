namespace SearchJobs.Api;

public static class DispatchServiceClientConfiguration
{
    public static IServiceCollection AddDispatchServiceClientConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var baseUrl = configuration.GetSection("DispatchService:BaseUrl").Get<string>()
            ?? throw new InvalidOperationException("Missing configuration value 'DispatchService__BaseUrl'.");

        services.AddHttpClient<IDispatchServiceClient, DispatchServiceClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });

        return services;
    }
}
