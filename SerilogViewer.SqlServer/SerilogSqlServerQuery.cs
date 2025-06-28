using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SerilogViewer.Abstractions;
using System.Diagnostics;

namespace SerilogViewer.SqlServer;

public class SerilogSqlServerQuery(
	TimestampType timestampType,
	ILogger<SerilogSqlServerQuery> logger,	
	LoggingRequestIdProvider requestIdProvider,
	string connectionString, string schemaName, string tableName) : SerilogQuery(timestampType)
{
	private readonly ILogger<SerilogSqlServerQuery> _logger = logger;
	private readonly LoggingRequestIdProvider _requestIdProvider = requestIdProvider;
	private readonly string _connectionString = connectionString;
	private readonly string _schemaName = schemaName;
	private readonly string _tableName = tableName;

	private class SerilogSqlServerEntry
	{
		public int Id { get; init; }
		public DateTime Timestamp { get; init; }
		public string? SourceContext { get; init; }
		public string? RequestId { get; init; }
		public string Level { get; init; } = default!;
		public string MessageTemplate { get; init; } = default!;
		public string Message { get; init; } = default!;
		public string? Exception { get; init; }
		public string? PropertyXml { get; init; }		
	}

	public override async Task<IEnumerable<SerilogEntry>> ExecuteAsync(Criteria? criteria = null, int offset = 0, int limit = 50)
	{
		using var cn = new SqlConnection(_connectionString);
		cn.Open();

		var (whereClause, parameters) = GetWhereClause(criteria, TimestampType);

		var query = 
			@$"SELECT 
				[Id], [Timestamp], [SourceContext], [RequestId], [Level], 
				[MessageTemplate], [Message], [Exception], [Properties] AS [PropertyXml]
			FROM [{_schemaName}].[{_tableName}] 
			{whereClause} 
			ORDER BY [Timestamp] DESC 
			OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY";

		_logger.BeginRequestId(_requestIdProvider.NextId());

		_logger.LogDebug("Querying serilog: {query}", query);

		var sw = Stopwatch.StartNew();
		bool error = false;
		try
		{
			var results = await cn.QueryAsync<SerilogSqlServerEntry>(query, parameters);

			// todo: apply non-query criteria (e.g., HasProperties, HassPropertyValues)

			return results.Select(ToSerilogEntry);
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

	private SerilogEntry ToSerilogEntry(SerilogSqlServerEntry source) => new()
	{
		Id = source.Id,
		Timestamp = source.Timestamp,
		SourceContext = source.SourceContext,
		RequestId = source.RequestId,
		Level = source.Level,
		MessageTemplate = source.MessageTemplate,
		Message = source.Message,
		Exception = source.Exception,
		Properties = string.IsNullOrEmpty(source.PropertyXml)
			? []
			: System.Xml.Linq.XElement.Parse(source.PropertyXml)
				.Elements()
				.ToDictionary(e => e.Attribute("key")!.Value, e => (object)e.Value)
	};
	

	private static (string, DynamicParameters? Parameters) GetWhereClause(Criteria? criteria, TimestampType timestampType)
	{
		if (criteria is null) return (string.Empty, null);

		List<string> terms = [];
		var parameters = new DynamicParameters();

		if (criteria.FromTimestamp.HasValue)
		{
			parameters.Add("@fromTimestamp", criteria.FromTimestamp.Value);
			terms.Add($"[Timestamp]>=@fromTimestamp");
		}

		if (criteria.ToTimestamp.HasValue)
		{
			parameters.Add("@toTimestamp", criteria.ToTimestamp.Value);
			terms.Add($"[Timestamp]<=@toTimestamp");
		}

		if (criteria.Age.HasValue)
		{
			var token = criteria.Age.Value switch
			{
				{ Days: > 0 } => "DAY",
				{ Hours: > 0 } => "HOUR",
				{ Minutes: > 0 } => "MINUTE",
				{ Seconds: > 0 } => "SECOND",
				_ => throw new ArgumentException("Unsupported age format")
			};

			var ageValue = criteria.Age.Value switch
			{
				{ Days: > 0 } => criteria.Age.Value.Days,
				{ Hours: > 0 } => criteria.Age.Value.Hours,
				{ Minutes: > 0 } => criteria.Age.Value.Minutes,
				{ Seconds: > 0 } => criteria.Age.Value.Seconds,
				_ => throw new ArgumentException("Unsupported age format")
			};

			var now = timestampType switch
			{
				TimestampType.Utc => "GETUTCDATE()",
				TimestampType.Local => "GETDATE()",
				_ => throw new ArgumentOutOfRangeException(nameof(timestampType))
			};

			terms.Add($"DATEDIFF({token}, [Timestamp], {now})<={ageValue}");
		}

		if (!string.IsNullOrEmpty(criteria.SourceContext))
		{
			parameters.Add("@sourceContext", criteria.SourceContext);
			terms.Add($"[SourceContext] LIKE '%' + @sourceContext + '%'");
		}

		if (!string.IsNullOrEmpty(criteria.RequestId))
		{
			parameters.Add("@requestId", criteria.RequestId);
			terms.Add($"[RequestId]=@requestId");
		}

		if (!string.IsNullOrEmpty(criteria.Level))
		{
			parameters.Add("@level", criteria.Level);
			terms.Add($"[Level]=@level");
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
