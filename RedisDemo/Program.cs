using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Net.Sockets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6739"; // Replace with your Redis server configuration
    options.InstanceName = "RedisDemo_"; // Optional prefix for Redis keys
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/cache-time", async (IDistributedCache cache) =>
{
    string cacheKey = "CachedTime";
    string? cachedTime = await cache.GetStringAsync(cacheKey);

    // If no cached value, store the current time
    if (cachedTime == null)
    {
        cachedTime = DateTime.UtcNow.ToString();
        await cache.SetStringAsync(cacheKey, cachedTime, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) // Cache expiration time
        });
    }
   

    return Results.Ok(new { CachedTime = cachedTime });
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
