using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SerilogBlazor.Abstractions;
using System.Data;

namespace SerilogBlazor.SqlServer;

public class SerilogSqlServerCleanup(
	LoggingRequestIdProvider requestIdProvider,	
	ILogger<SerilogSqlServerCleanup> logger,
	IOptions<SerilogCleanupOptions> options) : SerilogCleanup(requestIdProvider, logger, options)
{
	protected override Task<int> DeleteOldEntriesAsync(IDbConnection cn, string logLevel, int retentionDays)
	{
		var sql = $"DELETE FROM {Options.TableName} WHERE [Level] = @Level AND [Timestamp] < DATEADD(DAY, -@RetentionDays, GETUTCDATE())";
		return cn.ExecuteAsync(sql, new { Level = logLevel, RetentionDays = retentionDays }, commandTimeout: 0);
	}

	protected override IDbConnection GetConnection() => new SqlConnection(Options.ConnectionString);	
}
