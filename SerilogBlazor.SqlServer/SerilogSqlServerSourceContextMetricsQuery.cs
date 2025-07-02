using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SerilogBlazor.Abstractions;
using System.Diagnostics;

namespace SerilogBlazor.SqlServer;

public class SerilogSqlServerSourceContextMetricsQuery(
	TimestampType timestampType,
	ILogger<SerilogSqlServerSourceContextMetricsQuery> logger,
	LoggingRequestIdProvider requestIdProvider,
	string connectionString, string schemaName, string tableName) : SerilogSourceContextMetricsQuery
{
	private readonly TimestampType _timestampType = timestampType;
	private readonly ILogger<SerilogSqlServerSourceContextMetricsQuery> _logger = logger;
	private readonly LoggingRequestIdProvider _requestIdProvider = requestIdProvider;
	private readonly string _connectionString = connectionString;
	private readonly string _schemaName = schemaName;
	private readonly string _tableName = tableName;

	private class InternalSourceContextMetricsResult
	{
		public string SourceContext { get; init; } = default!;
		public string Level { get; init; } = default!;
		public DateTime LatestTimestamp { get; init; }
		public int Count { get; init; }
		public int AgeMinutes { get; init; }	
	}

	public override async Task<IEnumerable<SourceContextMetricsResult>> ExecuteAsync()
	{
		using var cn = new SqlConnection(_connectionString);
		cn.Open();

		var query =
			$@"WITH [source] AS (
				SELECT
					[SourceContext], [Level], MAX([Timestamp]) AS [LatestTimestamp], COUNT(1) AS [Count]
				FROM
					[log].[Serilog]
				WHERE
					[SourceContext] IS NOT NULL
				GROUP BY
					[SourceContext], [Level]
			)
			SELECT [src].*, DATEDIFF(n, [LatestTimestamp], GETDATE()) AS [AgeMinutes]
			FROM [source] AS [src]";

		_logger.BeginRequestId(_requestIdProvider.NextId());

		_logger.LogDebug("Querying serilog source context metrics: {query}", query);

		var sw = Stopwatch.StartNew();
		bool error = false;
		try
		{
			var results = await cn.QueryAsync<InternalSourceContextMetricsResult>(query);
			return results.Select(ToBaseType);
		}
		catch (Exception exc)
		{
			error = true;
			_logger.LogError(exc, "Error executing Serilog source context metrics query: {query}", query);
			throw;
		}
		finally
		{
			sw.Stop();
			_logger.LogInformation("Serilog source metrics query {status} in {elapsed} ms", error ? "failed" : "succeeded", sw.ElapsedMilliseconds);
		}		
	}

	private SourceContextMetricsResult ToBaseType(InternalSourceContextMetricsResult source) => new()
	{
		SourceContext = source.SourceContext,
		Level = source.Level,
		LatestTimestamp = source.LatestTimestamp,
		Count = source.Count,
		AgeText = DateHelper.ParseAgeText(source.AgeMinutes)
	};	
}
