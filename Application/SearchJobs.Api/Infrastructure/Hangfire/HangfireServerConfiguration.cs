using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Options;

namespace SearchJobs.Api;

public static class HangfireServerConfiguration
{
    public static IServiceCollection AddHangfireServerConfiguration(this IServiceCollection services)
    {
        services.AddHangfire((sp, configuration) =>
        {
            var connectionString = sp.GetRequiredService<IOptions<AppSettings>>().Value.ConnectionStrings.HangfireDb;

            configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(cfg => cfg.UseNpgsqlConnection(connectionString));
        });
        // DefaultRetryAttempts is 10
        GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 1, OnAttemptsExceeded = AttemptsExceededAction.Delete });

        services.AddHangfireServer();

        return services;
    }
}
