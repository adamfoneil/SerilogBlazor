using SerilogBlazor.Abstractions;

namespace SerilogBlazor.ApiConnector;

public interface IDetailQuery
{
	Task<SerilogEntry[]> ExecuteAsync(string? search, int offset, int rowCount);
}
