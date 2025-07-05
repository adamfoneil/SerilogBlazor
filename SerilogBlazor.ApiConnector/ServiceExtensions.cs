using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SerilogBlazor.Abstractions;

namespace SerilogBlazor.ApiConnector;

public static class ServiceExtensions
{
	public static void AddSerilogApiQueries(
		this IServiceCollection services, 
		Func<string?, Task<SerilogEntry[]>> detailQuery,
		Func<Task<SourceContextMetricsResult[]>> metricsQuery)
	{
		services.AddMemoryCache();
		services.AddSingleton(sp => new DetailQuery(detailQuery));
		services.AddSingleton(sp => new MetricsQuery(metricsQuery));
	}

	public static void MapSerilogEndpoints(this IEndpointRouteBuilder app, string path, string headerSecret, TimeSpan? cacheDuration = null)
	{
		cacheDuration ??= TimeSpan.FromMinutes(1);

		app.MapGet($"{path}/detail", async (ILogger<DetailQuery> logger, IMemoryCache cache, HttpRequest request, DetailQuery query, [FromQuery]string? search) =>
		{
			if (!ValidateHeaderSecret(request, headerSecret, logger)) return Results.Unauthorized();			

			try
			{				
				logger.LogDebug("Executing details query with search: '{Search}' with cache duration {duration}", search, cacheDuration);
				var (resultType, entries) = await cache.GetOrExecuteAsync(search.ToMd5(), async () => await query.ExecuteAsync(search), cacheDuration.Value);
				logger.LogDebug("Results were {ResultType}", resultType);
				return Results.Ok(entries);
			}
			catch (Exception exc)
			{
				logger.LogError(exc, "Error executing Serilog details query");
				return Results.Problem("Error executing Serilog details query");
			}
		});

		app.MapGet($"{path}/metrics", async (ILogger<MetricsQuery> logger, IMemoryCache cache, HttpRequest request, MetricsQuery query) =>
		{
			if (!ValidateHeaderSecret(request, headerSecret, logger)) return Results.Unauthorized();			

			try
			{
				logger.LogDebug("Executing metrics query with cache duration {duration}", cacheDuration);
				var (resultType, metrics) = await cache.GetOrExecuteAsync("metrics", async () => await query.ExecuteAsync(), cacheDuration.Value);
				logger.LogDebug("Results were {ResultType}", resultType);
				return Results.Ok(metrics);
			}
			catch (Exception exc)
			{
				logger.LogError(exc, "Error executing Serilog metrics query");
				return Results.Problem("Error executing Serilog metrics query");
			}
		});
	}

	private static bool ValidateHeaderSecret<T>(HttpRequest request, string headerSecret, ILogger<T> logger)
	{		
		if (request.Headers.TryGetValue("serilog-api-secret", out var secretValue))
		{			
			var result = secretValue == headerSecret;
			logger.LogDebug("Header secret found: {Secret}, match = {match}", secretValue, result);
			return result;
		}

		logger.LogWarning("Header secret not found or does not match");
		return false;
	}
}
