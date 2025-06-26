using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace SerilogViewer.Abstractions;

/// <summary>
/// Define logging levels for different namespaces that can be toggled at runtime
/// </summary>
public abstract class LogLevels
{
	public abstract Dictionary<string, LogEventLevel> LoggingLevels { get; }

	public LoggerConfiguration GetConfiguration(LogEventLevel minimumLevel = LogEventLevel.Information)
	{
		var loggerConfig = new LoggerConfiguration()
			.MinimumLevel.ControlledBy(new LoggingLevelSwitch(minimumLevel));

		foreach (var kp in LoggingLevels)
		{
			loggerConfig = loggerConfig.MinimumLevel.Override(kp.Key, new LoggingLevelSwitch(kp.Value));
		}

		return loggerConfig;
	}
}
