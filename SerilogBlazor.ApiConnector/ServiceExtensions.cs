using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SerilogBlazor.Abstractions;

namespace SerilogBlazor.ApiConnector;

public static class ServiceExtensions
{
	public static IEndpointRouteBuilder MapSerilogEndpoints(this IEndpointRouteBuilder app, string path, string headerSecret, TimeSpan? cacheDuration = null)
	{
		cacheDuration ??= TimeSpan.FromMinutes(1);

		app.MapGet($"{path}/detail", async ([FromServices]ILogger<IDetailQuery> logger, [FromServices]IMemoryCache cache, HttpRequest request, [FromServices] IDetailQuery query, 
			[FromQuery]string? search, [FromQuery]int? offset, [FromQuery]int? rowCount) =>
		{
			if (!ValidateHeaderSecret(request, headerSecret, logger)) return Results.Unauthorized();

			offset ??= 0;
			rowCount ??= 50;

			var key = new
			{
				offset,
				rowCount,
				Search = search ?? "empty"
			}.ToMd5();

			try
			{				
				logger.LogDebug("Executing details query with search: '{Search}' with cache duration {duration}", search, cacheDuration);
				var (resultType, entries) = await cache.GetOrExecuteAsync(key, async () => await query.ExecuteAsync(search, offset.Value, rowCount.Value), cacheDuration.Value);
				logger.LogDebug("Results were {ResultType}", resultType);
				return Results.Ok(entries);
			}
			catch (Exception exc)
			{
				logger.LogError(exc, "Error executing Serilog details query");
				return Results.Problem("Error executing Serilog details query");
			}
		});

		app.MapGet($"{path}/metrics", async ([FromServices] ILogger<IMetricsQuery> logger, [FromServices] IMemoryCache cache, HttpRequest request, [FromServices] IMetricsQuery query) =>
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

		app.MapGet($"{path}/levels", ([FromServices]ILogLevels logLevels, ILogger<ILogLevels> logger, HttpRequest request) =>
		{
			if (!ValidateHeaderSecret(request, headerSecret, logger)) return Results.Unauthorized();

			logger.LogDebug("Getting log levels configuration");

			return Results.Ok(new
			{
				DefaultLevel = logLevels.DefaultLevelSwitch.MinimumLevel.ToString(),
				ConfiguredLevels = logLevels.LoggingLevels.ToDictionary(kp => kp.Key, kp => kp.Value.MinimumLevel.ToString())
			});
		});

		return app;
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
