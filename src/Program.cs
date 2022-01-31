// Copyright (c) EverRise Pte Ltd. All rights reserved.

using EverStats.Services;
using EverStats.Config;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var config = ApiConfig.GetConfiguration();
var stats = new Stats(config);
builder.Services
    .AddSingleton<ApiConfig>(config)
    .AddSingleton<Stats>(stats)
    .AddHostedService(provider => stats)
    .AddHostedService<TwitterBot>();

var app = builder.Build();

app.UseStats();
app.MapGet("/", () => "Hello World!");
app.UseStaticFiles();

app.Run();
