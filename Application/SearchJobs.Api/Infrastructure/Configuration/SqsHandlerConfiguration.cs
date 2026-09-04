using Amazon.Runtime;
using Amazon.SQS;
namespace SearchJobs.Api;

public static class SqsHandlerConfiguration
{
    public static IServiceCollection AddSqsHandlerConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var awsOptions = configuration.GetAWSOptions();
        awsOptions.Credentials = new BasicAWSCredentials("", "");

        services.AddDefaultAWSOptions(awsOptions);
        services.AddAWSService<IAmazonSQS>();

        return services;
    }
}
