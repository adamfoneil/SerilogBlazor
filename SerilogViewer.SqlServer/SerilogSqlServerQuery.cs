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
		public int AgeMinutes { get; init; } // calculated as DATEDIFF(n, Timestamp, {c})		
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

		var (whereClause, parameters, _) = GetWhereClause(criteria, TimestampType);

		var query = 
			@$"SELECT 
				[Id], [Timestamp], DATEDIFF(n, [Timestamp], {CurrentTimeFunction(TimestampType)}) AS [AgeMinutes], 
				[SourceContext], [RequestId], [Level], [MessageTemplate], 
				[Message], [Exception], [Properties] AS [PropertyXml]
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

	protected override IEnumerable<string> GetSearchTerms(Criteria criteria)
	{
		var (_, _, searchTerms) = GetWhereClause(criteria, TimestampType);
		return searchTerms;
	}

	private SerilogEntry ToSerilogEntry(SerilogSqlServerEntry source) => new()
	{
		Id = source.Id,
		Timestamp = source.Timestamp,
		AgeText = ParseAgeText(source.AgeMinutes),
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

	private static string ParseAgeText(int ageMinutes)
	{
		if (ageMinutes < 1)
			return "just now";

		var ts = TimeSpan.FromMinutes(ageMinutes);

		var parts = new List<string>();

		if (ts.Days > 0)
			parts.Add($"{ts.Days}d");
		if (ts.Hours > 0)
			parts.Add($"{ts.Hours}h");
		if (ts.Minutes > 0 && ts.Days == 0) // Only show minutes if less than a day
			parts.Add($"{ts.Minutes}m");

		return string.Join(", ", parts);
	}


	private static string CurrentTimeFunction(TimestampType timestampType) => timestampType switch
	{
		TimestampType.Utc => "GETUTCDATE()",
		TimestampType.Local => "GETDATE()",
		_ => throw new ArgumentOutOfRangeException(nameof(timestampType))
	};

	private static (string, DynamicParameters? Parameters, IEnumerable<string> SearchTerms) GetWhereClause(Criteria? criteria, TimestampType timestampType)
	{
		if (criteria is null) return (string.Empty, null, []);

		List<(string Sql, string Display)> terms = [];
		var parameters = new DynamicParameters();

		if (criteria.FromTimestamp.HasValue)
		{
			parameters.Add("@fromTimestamp", criteria.FromTimestamp.Value);
			terms.Add(($"[Timestamp]>=@fromTimestamp", $"Timestamp after {criteria.FromTimestamp}"));
		}

		if (criteria.ToTimestamp.HasValue)
		{
			parameters.Add("@toTimestamp", criteria.ToTimestamp.Value);
			terms.Add(($"[Timestamp]<=@toTimestamp", $"Timestamp before {criteria.ToTimestamp}"));
		}

		if (criteria.Age.HasValue)
		{
			var ts = criteria.Age.Value;
			string token;
			int ageValue;

			// Determine the most appropriate time unit for DATEDIFF
			if (ts.Days >= 30 && ts.Days % 30 == 0)
			{
				// Use months for multiples of 30 days
				token = "mm";
				ageValue = ts.Days / 30;
			}
			else if (ts.Days >= 7 && ts.Days % 7 == 0)
			{
				// Use weeks for multiples of 7 days
				token = "wk";
				ageValue = ts.Days / 7;
			}
			else if (ts.Days > 0)
			{
				// Use days
				token = "d";
				ageValue = ts.Days;
			}
			else if (ts.Hours > 0)
			{
				// Use hours
				token = "hh";
				ageValue = ts.Hours;
			}
			else if (ts.Minutes > 0)
			{
				// Use minutes
				token = "n";
				ageValue = ts.Minutes;
			}
			else if (ts.Seconds > 0)
			{
				// Use seconds
				token = "s";
				ageValue = ts.Seconds;
			}
			else
			{
				throw new ArgumentException("Unsupported age format");
			}

			var dateDiff = $"DATEDIFF({token}, [Timestamp], {CurrentTimeFunction(timestampType)})";
			terms.Add(($"{dateDiff}<={ageValue}", $"At most {ageValue} {token} ago"));
		}

		if (!string.IsNullOrEmpty(criteria.SourceContext))
		{
			parameters.Add("@sourceContext", criteria.SourceContext);
			terms.Add(($"[SourceContext] LIKE '%' + @sourceContext + '%'", $"Source context contains '{criteria.SourceContext}'"));
		}

		if (!string.IsNullOrEmpty(criteria.RequestId))
		{
			parameters.Add("@requestId", criteria.RequestId);
			terms.Add(($"[RequestId]=@requestId", $"Request Id = {criteria.RequestId}"));
		}

		if (!string.IsNullOrEmpty(criteria.Level))
		{
			parameters.Add("@level", criteria.Level);
			terms.Add(($"[Level]=@level", $"Level = {criteria.Level}"));
		}

		if (!string.IsNullOrEmpty(criteria.Message))
		{
			parameters.Add("@message", criteria.Message);
			terms.Add(($"[Message] LIKE '%' + @message + '%'", $"Message contains '{criteria.Message}'"));
		}

		if (!string.IsNullOrEmpty(criteria.Exception))
		{
			parameters.Add("@exception", criteria.Exception);
			terms.Add(($"[Exception] LIKE '%' + @exception + '%'", $"Exception contains '{criteria.Exception}'"));
		}

		return 
			(terms.Any() ? $"WHERE {string.Join(" AND ", terms.Select(item => item.Sql))}" : string.Empty, 
			parameters, 
			terms.Select(item => item.Display));
	}	
}
