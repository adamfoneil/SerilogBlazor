using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SerilogBlazor.Abstractions;
using Serilog.Events;

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

			var dto = new LogLevelsDto
			{
				DefaultLevel = logLevels.DefaultLevelSwitch.MinimumLevel.ToString(),
				ConfiguredLevels = logLevels.LoggingLevels.ToDictionary(kp => kp.Key, kp => kp.Value.MinimumLevel.ToString())
			};

			return Results.Ok(dto);
		});

		app.MapPut($"{path}/levels", ([FromServices]ILogLevels logLevels, ILogger<ILogLevels> logger, HttpRequest request, [FromBody]LogLevelsDto dto) =>
		{
			if (!ValidateHeaderSecret(request, headerSecret, logger)) return Results.Unauthorized();

			logger.LogDebug("Updating log levels configuration");

			try
			{
				// Update default level
				if (Enum.TryParse<LogEventLevel>(dto.DefaultLevel, out var defaultLevel))
				{
					logLevels.DefaultLevelSwitch.MinimumLevel = defaultLevel;
					logger.LogDebug("Updated default level to {Level}", defaultLevel);
				}
				else
				{
					logger.LogWarning("Invalid default level: {Level}", dto.DefaultLevel);
					return Results.BadRequest($"Invalid default level: {dto.DefaultLevel}");
				}

				// Update configured levels
				foreach (var kvp in dto.ConfiguredLevels)
				{
					if (logLevels.LoggingLevels.TryGetValue(kvp.Key, out var levelSwitch))
					{
						if (Enum.TryParse<LogEventLevel>(kvp.Value, out var level))
						{
							levelSwitch.MinimumLevel = level;
							logger.LogDebug("Updated level for {Namespace} to {Level}", kvp.Key, level);
						}
						else
						{
							logger.LogWarning("Invalid level for {Namespace}: {Level}", kvp.Key, kvp.Value);
							return Results.BadRequest($"Invalid level for {kvp.Key}: {kvp.Value}");
						}
					}
					else
					{
						logger.LogWarning("Unknown namespace: {Namespace}", kvp.Key);
						return Results.BadRequest($"Unknown namespace: {kvp.Key}");
					}
				}

				logger.LogInformation("Log levels configuration updated successfully");
				return Results.Ok("Log levels updated successfully");
			}
			catch (Exception exc)
			{
				logger.LogError(exc, "Error updating log levels configuration");
				return Results.Problem("Error updating log levels configuration");
			}
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
