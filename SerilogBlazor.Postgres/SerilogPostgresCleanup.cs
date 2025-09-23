using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using SerilogBlazor.Abstractions;
using System.Data;

namespace SerilogBlazor.Postgres;

public class SerilogPostgresCleanup(
	string timezone,
	LoggingRequestIdProvider requestIdProvider,
	ILogger<SerilogPostgresCleanup> logger,
	IOptions<SerilogCleanupOptions> options) : SerilogCleanup(requestIdProvider, logger, options)
{
	private readonly string _timezone = timezone;

	protected override Task<int> DeleteOldEntriesAsync(IDbConnection cn, string logLevel, int retentionDays)
	{
		// Convert string level to int for Postgres
		var sql = $@"DELETE FROM ""{Options.TableName}"" 
			WHERE ""level"" = @Level 
			AND ""timestamp"" < (NOW() AT TIME ZONE '{_timezone}' - INTERVAL '{retentionDays} days')";
		
		return cn.ExecuteAsync(sql, new { Level = logLevel, RetentionDays = retentionDays }, commandTimeout: 0);
	}

	protected override IDbConnection GetConnection() => new NpgsqlConnection(Options.ConnectionString);
}