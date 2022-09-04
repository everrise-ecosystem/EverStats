// Copyright (c) EverRise Pte Ltd. All rights reserved.

using EverStats.Services;
using EverStats.Config;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
var config = ApiConfig.GetConfiguration();
builder.Services
    .AddSingleton<ApiConfig>(config)
    .AddSingleton<HolderList>()
    //.AddSingleton<QueryTime>()
    //.AddHostedService(provider => provider.GetRequiredService<QueryTime>())
    //.AddSingleton<Stats>()
    //.AddHostedService(provider => provider.GetRequiredService<Stats>())
    .AddHostedService<TwitterBot>()
    .AddControllers()
    ;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    config.StoreInDb = true;
    config.SendTweets = false;
} else
{
    config.StoreInDb = false;
    config.SendTweets = true;
}

app.MapGet("/time", () => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

app.UseHttpsRedirection();
app.MapControllers();

//app.UseStats();
//app.UseCoinPricing();
app.UseStaticFiles();

app.Run();
