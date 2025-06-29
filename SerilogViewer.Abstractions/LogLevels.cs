using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace SerilogViewer.Abstractions;

/// <summary>
/// Define logging levels for different namespaces that can be toggled at runtime
/// </summary>
public abstract class LogLevels(LogEventLevel defaultMinLevel = LogEventLevel.Information)
{
	public abstract Dictionary<string, LoggingLevelSwitch> LoggingLevels { get; }

	public readonly LogEventLevel DefaultLevel = defaultMinLevel;

	public LoggerConfiguration GetConfiguration()
	{
		var loggerConfig = new LoggerConfiguration()
			.MinimumLevel.ControlledBy(new LoggingLevelSwitch(DefaultLevel));

		foreach (var kp in LoggingLevels)
		{
			loggerConfig = loggerConfig.MinimumLevel.Override(kp.Key, kp.Value);
		}

		return loggerConfig;
	}
}
