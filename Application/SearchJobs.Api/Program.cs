using Hangfire;
using SearchJobs.Api;
using SearchJobs.Api.Interfaces;
using DotNetEnv;
using SearchJobs.Api.JobProcessors.DispatchQueueProcessor;

Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env"));

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddHangfireServerConfiguration(builder.Configuration);


builder.Services.AddSqsHandlerConfiguration(builder.Configuration);
builder.Services.AddDispatchSearchServiceClientConfiguration(builder.Configuration);
builder.Services.AddDispatchServiceClientConfiguration(builder.Configuration);

builder.Services.AddTransient(typeof(IJobEnqueuer<>), typeof(HangfireJobEnqueuer<>));
builder.Services.AddTransient<IDispatchServiceMessagesHandler, SqsMessagesHandler>();
builder.Services.AddTransient<IDispatchJobProcessor, DispatchJobProcessor>();
builder.Services.AddTransient<ICheckpointStore, HangfireCheckpointStore>();
builder.Services.AddSingleton<SqsPollingBackgroundService>();
builder.Services.AddHostedService<SqsPollingBackgroundService>();

builder.Services.AddControllers();
builder.Services.AddTransient<IBackfillJob, DispatchBackfillJob>();

var app = builder.Build();

app.UseHangfireDashboard();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();