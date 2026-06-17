using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace DigitalWalletAndLedgerAPI.Helpers;

public class IdempotencyFilter : IEndpointFilter
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<IdempotencyFilter> _logger;

    public IdempotencyFilter(IDistributedCache cache, ILogger<IdempotencyFilter> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        if (!httpContext.Request.Headers.TryGetValue("X-Idempotency-Key", out var extractedKey))
        {
            return await next(context);
        }

        var idempotencyKey = extractedKey.ToString();

        var cachedResponse = await _cache.GetStringAsync(idempotencyKey);
        if (!string.IsNullOrEmpty(cachedResponse))
        {
            _logger.LogInformation("Idempotency match found in Redis for Key: {IdempotencyKey}. Short-circuiting request.", idempotencyKey);
            var savedResult = JsonSerializer.Deserialize<JsonElement>(cachedResponse);
            return Results.Ok(savedResult);
        }

        var result = await next(context);
        if (httpContext.Response.StatusCode is >= 200 and < 300 && result is IResult okResult)
        {
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            };
            if (okResult is Microsoft.AspNetCore.Http.HttpResults.Ok<object> dynamicOk)
            {
                var serializedPayload = JsonSerializer.Serialize(dynamicOk.Value);
                await _cache.SetStringAsync(idempotencyKey, serializedPayload, cacheOptions);
            }
        }

        return result;
    }
}