using Hangfire;
using Hangfire.PostgreSql;

namespace SearchJobs.Api;

public static class HangfireServerConfiguration
{
    public static IServiceCollection AddHangfireServerConfiguration(this IServiceCollection services, IConfiguration globalConfiguration)
    {
        var connectionString = globalConfiguration.GetSection("ConnectionStrings:HangfireDb").Get<string>();

        services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(configuration => configuration.UseNpgsqlConnection(connectionString))
        );

        services.AddHangfireServer();

        return services;
    }
}
