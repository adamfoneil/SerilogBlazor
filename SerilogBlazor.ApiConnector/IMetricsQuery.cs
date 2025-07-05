using SerilogBlazor.Abstractions;

namespace SerilogBlazor.ApiConnector;

public interface IMetricsQuery
{
	Task<SourceContextMetricsResult[]> ExecuteAsync();
}
