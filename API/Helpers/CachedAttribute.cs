using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Helpers;

public class CachedAttribute : Attribute, IAsyncActionFilter
{
    private readonly int timeToLiveSeconds;

    public CachedAttribute(int timeToLiveSeconds)
    {
        this.timeToLiveSeconds = timeToLiveSeconds;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        IResponseCacheService cacheService =
            context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

        string cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
        string? cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedResponse))
        {
            ContentResult contentResult = new ContentResult
            {
                Content = cachedResponse,
                ContentType = "application/json",
                StatusCode = 200
            };

            context.Result = contentResult;
            return;
        }

        // Move to controller.
        ActionExecutedContext executedContext = await next();

        if (executedContext.Result == null
            || executedContext.Result is not OkObjectResult okObjectResult
            || okObjectResult.Value == null)
        {
            return;
        }

        await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(timeToLiveSeconds));
    }

    private string GenerateCacheKeyFromRequest(HttpRequest request)
    {
        StringBuilder keyBuilder = new StringBuilder();
        keyBuilder.Append($"{request.Path}");
        Func<KeyValuePair<string, StringValues>, string> getKey =
            (KeyValuePair<string, StringValues> param) => param.Key;


        foreach ((string key, StringValues value) in request.Query.OrderBy(getKey))
        {
            keyBuilder.Append($"|{key}-{value}");
        }

        return keyBuilder.ToString();
    }
}
