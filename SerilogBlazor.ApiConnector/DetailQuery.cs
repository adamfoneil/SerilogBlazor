using SerilogBlazor.Abstractions;

namespace SerilogBlazor.ApiConnector;

internal class DetailQuery(Func<string?, Task<SerilogEntry[]>> query)
{
	private readonly Func<string?, Task<SerilogEntry[]>> _query = query;

	public async Task<SerilogEntry[]> ExecuteAsync(string? search) => await _query(search);
}
