using SerilogBlazor.Abstractions;

namespace SerilogBlazor.ApiConnector;

internal class MetricsQuery(Func<Task<SourceContextMetricsResult[]>> query)
{
	private readonly Func<Task<SourceContextMetricsResult[]>> _query = query;

	public async Task<SourceContextMetricsResult[]> ExecuteAsync() => await _query();
}
