
using ApiGateway;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

builder.Services.AddTransient<IHttpMessageHandlerBuilderFilter, CircuitBreakerHandlerFilter>();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.Use(async (context, next) =>
{
    var cache = context.RequestServices.GetRequiredService<IMemoryCache>();
    var cacheKey = context.Request.Path;
    
    if (context.Request.Method == "GET" && cache.TryGetValue(cacheKey, out var cachedResponse))
    {
        await context.Response.WriteAsJsonAsync(cachedResponse);
        return;
    }
    
    var originalBody = context.Response.Body;
    using var memStream = new MemoryStream();
    context.Response.Body = memStream;

    await next();

    if (context.Response.StatusCode == 200)
    {
        memStream.Position = 0;
        var response = await new StreamReader(memStream).ReadToEndAsync();

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(10));
    
        cache.Set(cacheKey, response, cacheOptions);
    }

    memStream.Position = 0;
    await memStream.CopyToAsync(originalBody);
});

app.MapReverseProxy();

app.Run();

