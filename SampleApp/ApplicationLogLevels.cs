using Serilog.Core;
using Serilog.Events;
using SerilogBlazor.Abstractions;

namespace SampleApp;

public class ApplicationLogLevels : LogLevels
{
	/// <summary>
	/// Set your defaults as a field initialized once so that values can be changed at runtime.
	/// Changes at runtime won't persist across app restarts, but will allow for dynamic adjustments during an app's lifetime.
	/// </summary>
	private readonly Dictionary<string, LoggingLevelSwitch> _loggingLevels = new()
	{
		["System"] = new(LogEventLevel.Warning),
		["Microsoft"] = new(LogEventLevel.Warning),
		["SampleApp"] = new(LogEventLevel.Information),
		["SerilogBlazor.SqlServer"] = new(LogEventLevel.Information)
	};

	public override Dictionary<string, LoggingLevelSwitch> LoggingLevels => _loggingLevels;
}
