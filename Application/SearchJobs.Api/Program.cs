using Hangfire;
using SearchJobs.Api;
using SearchJobs.Api.Interfaces;
using DotNetEnv;

Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env"));

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddHangfireServerConfiguration(builder.Configuration);


builder.Services.AddSqsHandlerConfiguration(builder.Configuration);

builder.Services.AddTransient<IMessagesHandler, SqsMessagesHandler>();
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