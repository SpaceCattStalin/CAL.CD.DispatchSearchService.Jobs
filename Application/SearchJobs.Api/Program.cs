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

builder.Services.AddTransient(typeof(IJobEnqueuer<>), typeof(HangfireJobEnqueuer<>));
builder.Services.AddTransient<IMessagesHandler, SqsMessagesHandler>();
builder.Services.AddTransient<IDispatchJobProcessor, DispatchJobProcessor>();
builder.Services.AddSingleton<SqsPollingBackgroundService>();
builder.Services.AddHostedService<SqsPollingBackgroundService>();

var app = builder.Build();

app.UseHangfireDashboard();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();