using SerilogBlazor.Abstractions;

namespace SerilogBlazor.ApiConnector;

public abstract class MetricsQuery
{
	public abstract Task<SourceContextMetricsResult[]> ExecuteAsync();
}
