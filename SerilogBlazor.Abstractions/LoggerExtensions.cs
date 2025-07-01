using Microsoft.Extensions.Logging;

namespace SerilogBlazor.Abstractions;

public static class LoggerExtensions
{
	/// <summary>
	/// attaches the given requestId to the logger scope, so you can correlate logs with a specific request
	/// </summary>
	public static IDisposable? BeginRequestId<T>(this ILogger<T> logger, string requestId) => 
		logger.BeginScope(new Dictionary<string, object>
		{
			["RequestId"] = requestId
		});

	/// <summary>
	/// attaches the next incremented requestId to the logger
	/// </summary>
	public static IDisposable? BeginRequestId<T>(this ILogger<T> logger, LoggingRequestIdProvider idProvider) => 
		BeginRequestId<T>(logger, idProvider.NextId());
}
