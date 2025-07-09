using static SerilogBlazor.Abstractions.SerilogQuery;

namespace SerilogBlazor.Abstractions;

public interface ISerilogQuery
{
	Task<IEnumerable<SerilogEntry>> ExecuteAsync(string? query = null, int offset = 0, int limit = 50);	
}
