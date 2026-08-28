using Hangfire;
using SearchJobs.Api;
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
builder.Services.AddSingleton<SqsPollingBackgroundService>();
builder.Services.AddHostedService<SqsPollingBackgroundService>();

builder.Services.AddControllers();
builder.Services.AddTransient<ISyncJob, DispatchSyncJob>();

var app = builder.Build();

app.UseHangfireDashboard();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();