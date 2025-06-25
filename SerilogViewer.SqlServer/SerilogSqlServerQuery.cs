using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SerilogViewer.Abstractions;
using System.Diagnostics;

namespace SerilogViewer.SqlServer;

public class SerilogSqlServerQuery(
	ILogger<SerilogSqlServerQuery> logger,
	string connectionString, string schemaName, string tableName) : SerilogQuery
{
	private readonly ILogger<SerilogSqlServerQuery> _logger = logger;
	private readonly string _connectionString = connectionString;
	private readonly string _schemaName = schemaName;
	private readonly string _tableName = tableName;

	private static int _nextRequestId = 0;

	public override async Task<IEnumerable<SerilogEntry>> ExecuteAsync(Criteria? criteria = null, int offset = 0, int limit = 50)
	{
		using var cn = new SqlConnection(_connectionString);
		cn.Open();

		var (whereClause, parameters) = GetWhereClause(criteria);

		var query = 
			@$"SELECT * 
			FROM [{_schemaName}].[{_tableName}] 
			{whereClause} 
			ORDER BY [Timestamp] DESC 
			OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY";

		_logger.RequestId($"request{++_nextRequestId}");

		_logger.LogDebug("Querying serilog: {query}", query);

		var sw = Stopwatch.StartNew();
		bool error = false;
		try
		{
			var results = await cn.QueryAsync<SerilogEntry>(query, parameters);

			// todo: apply non-query criteria (e.g., HasProperties, HassPropertyValues)

			return results;
		}
		catch (Exception exc)
		{
			error = true;
			_logger.LogError(exc, "Error executing Serilog query: {query}", query);
			throw;
		}
		finally
		{
			sw.Stop();
			_logger.LogInformation("Serilog query {status} in {elapsed} ms", error ? "failed" : "succeeded", sw.ElapsedMilliseconds);
		}
	}

	private static (string, DynamicParameters? Parameters) GetWhereClause(Criteria? criteria)
	{
		if (criteria is null) return (string.Empty, null);

		List<string> terms = [];
		var parameters = new DynamicParameters();

		if (criteria.FromTimestamp.HasValue)
		{
			parameters.Add("@fromTimestamp", criteria.FromTimestamp.Value);
			terms.Add($"[Timestamp] >= @fromTimestamp");
		}

		if (criteria.ToTimestamp.HasValue)
		{
			parameters.Add("@toTimestamp", criteria.ToTimestamp.Value);
			terms.Add($"[Timestamp] <= @toTimestamp");
		}

		if (!string.IsNullOrEmpty(criteria.SourceContext))
		{
			parameters.Add("@sourceContext", criteria.SourceContext);
			terms.Add($"[SourceContext] = @sourceContext");
		}

		if (!string.IsNullOrEmpty(criteria.RequestId))
		{
			parameters.Add("@requestId", criteria.RequestId);
			terms.Add($"[RequestId] = @requestId");
		}

		if (!string.IsNullOrEmpty(criteria.Level))
		{
			parameters.Add("@level", criteria.Level);
			terms.Add($"[Level] = @level");
		}

		if (!string.IsNullOrEmpty(criteria.Message))
		{
			parameters.Add("@message", criteria.Message);
			terms.Add($"[Message] LIKE '%' + @message + '%'");
		}

		if (!string.IsNullOrEmpty(criteria.Exception))
		{
			parameters.Add("@exception", criteria.Exception);
			terms.Add($"[Exception] LIKE '%' + @exception + '%'");
		}

		return (terms.Any() ? $"WHERE {string.Join(" AND ", terms)}" : string.Empty, parameters);
	}
}
