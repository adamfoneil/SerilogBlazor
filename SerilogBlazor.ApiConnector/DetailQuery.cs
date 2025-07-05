using SerilogBlazor.Abstractions;

namespace SerilogBlazor.ApiConnector;

public abstract class DetailQuery
{
	public abstract Task<SerilogEntry[]> ExecuteAsync(string? search);
}
