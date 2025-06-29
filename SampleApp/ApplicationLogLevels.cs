using Serilog.Core;
using Serilog.Events;
using SerilogViewer.Abstractions;

namespace SampleApp;

public class ApplicationLogLevels : LogLevels
{
	public override Dictionary<string, LoggingLevelSwitch> LoggingLevels => new()
	{
		["System"] = new(LogEventLevel.Warning),
		["Microsoft"] = new(LogEventLevel.Warning),
		["SampleApp"] = new(LogEventLevel.Debug),
		["SerilogViewer.SqlServer"] = new(LogEventLevel.Debug)
	};
}
