using Serilog.Events;
using SerilogViewer.Abstractions;

namespace SampleApp;

public class ApplicationLogLevels : LogLevels
{
	public override Dictionary<string, LogEventLevel> LoggingLevels => new()
	{
		["System"] = LogEventLevel.Warning,
		["Microsoft"] = LogEventLevel.Warning,
		["SampleApp"] = LogEventLevel.Debug,
		["SerilogViewer.SqlServer"] = LogEventLevel.Debug
	};
}
