using ApiGateway;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.All;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddMemoryCache();
builder.Services.AddTransient<IHttpMessageHandlerBuilderFilter, CircuitBreakerHandlerFilter>();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("GatewayCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); 
    });
});
var app = builder.Build();
app.UseForwardedHeaders();
app.UseCors("GatewayCorsPolicy");
app.Use(async (context, next) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        await next();
        return;
    }

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