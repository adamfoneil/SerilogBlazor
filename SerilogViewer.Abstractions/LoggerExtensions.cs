using Microsoft.Extensions.Logging;

namespace SerilogViewer.Abstractions;

public static class LoggerExtensions
{
	public static IDisposable? RequestId<T>(this ILogger<T> logger, string requestId) => 
		logger.BeginScope(new Dictionary<string, object>
		{
			["RequestId"] = requestId
		});
}
