using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using SerilogBlazor.Abstractions;
using System.Data;

namespace SerilogBlazor.Postgres;

public class SerilogPostgresCleanup(
	LoggingRequestIdProvider requestIdProvider,
	ILogger<SerilogPostgresCleanup> logger,
	IOptions<SerilogCleanupOptions> options) : SerilogCleanup(requestIdProvider, logger, options)
{
	protected override Task<int> DeleteOldEntriesAsync(IDbConnection cn, string logLevel, int retentionDays)
	{
		// Convert string level to int for Postgres
		var levelInt = PostgresHelpers.LevelStringToInt(logLevel);
		
		var sql = $@"DELETE FROM ""{Options.TableName}"" 
			WHERE ""Level"" = @Level 
			AND ""Timestamp"" < (NOW() AT TIME ZONE 'UTC' - INTERVAL '{retentionDays} days')";
		
		return cn.ExecuteAsync(sql, new { Level = levelInt, RetentionDays = retentionDays }, commandTimeout: 0);
	}

	protected override IDbConnection GetConnection() => new NpgsqlConnection(Options.ConnectionString);
}