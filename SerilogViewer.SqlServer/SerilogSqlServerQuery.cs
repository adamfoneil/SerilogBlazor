using SerilogViewer.Abstractions;

namespace SerilogViewer.SqlServer;

public class SerilogSqlServerQuery(string connectionString) : SerilogQuery
{
	private readonly string _connectionString = connectionString;

	public override Task<SerilogEntry[]> ExecuteAsync(Criteria? criteria = null, int offset = 0, int limit = 50)
	{
		throw new NotImplementedException();
	}
}
