using System.Text;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SearchJobs.Api;
using SearchJobs.Api.Authentication;
using SearchJobs.Api.Core;
using SearchJobs.Api.Interfaces;
using SearchJobs.Api.JobProcessors.DispatchQueueProcessor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddOptions<AppSettings>()
    .Bind(builder.Configuration)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddHangfireServerConfiguration();

builder.Services.AddSqsHandlerConfiguration(builder.Configuration);
builder.Services.AddDispatchSearchServiceClientConfiguration();
builder.Services.AddDispatchServiceClientConfiguration();

builder.Services.AddTransient(typeof(IJobEnqueuer<>), typeof(HangfireJobEnqueuer<>));
builder.Services.AddTransient<IDispatchServiceMessagesHandler, SqsMessagesHandler>();
builder.Services.AddTransient<IDispatchJobProcessor, DispatchJobProcessor>();
builder.Services.AddTransient<ICheckpointStore, HangfireCheckpointStore>();
//builder.Services.AddSingleton<SqsPollingBackgroundService>();
builder.Services.AddHostedService<SqsPollingBackgroundService>();

builder.Services.AddControllers();
builder.Services.AddTransient<ISyncJob, DispatchSyncJob>();
builder.Services.AddTransient<FailJobCleanupWorker>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<AppSettings>>((options, appSettings) =>
    {
        var jwt = appSettings.Value.Jwt;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
            ValidateLifetime = true
        };
    });
    
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("sync:update-all", policy =>
       policy.Requirements.Add(new PermissionAuthorizationRequirement("sync:update-all")));
});

var app = builder.Build();

app.UseHangfireDashboard();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();