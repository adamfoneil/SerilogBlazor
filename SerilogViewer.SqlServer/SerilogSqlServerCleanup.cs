using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SerilogViewer.Abstractions;
using System.Data;

namespace SerilogViewer.SqlServer;

public class SerilogSqlServerCleanup(
	string connectionString,
	ILogger<SerilogSqlServerCleanup> logger,
	IOptions<SerilogCleanupOptions> options) : SerilogCleanup(logger, options)
{
	private readonly string _connectionString = connectionString;

	protected override Task<int> DeleteOldEntriesAsync(IDbConnection cn, string logLevel, int retentionDays)
	{
		throw new NotImplementedException();
	}

	protected override IDbConnection GetConnection() => new SqlConnection(_connectionString);	
}
