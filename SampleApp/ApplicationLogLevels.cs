using Serilog.Core;
using Serilog.Events;
using SerilogViewer.Abstractions;

namespace SampleApp;

public class ApplicationLogLevels : LogLevels
{
	private readonly Dictionary<string, LoggingLevelSwitch> _loggingLevels = new()
	{
		["System"] = new(LogEventLevel.Warning),
		["Microsoft"] = new(LogEventLevel.Warning),
		["SampleApp"] = new(LogEventLevel.Debug),
		["SerilogViewer.SqlServer"] = new(LogEventLevel.Debug)
	};

	public override Dictionary<string, LoggingLevelSwitch> LoggingLevels => _loggingLevels;
}
