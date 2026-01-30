using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using SerilogBlazor.Abstractions;
using System.Diagnostics;
using System.Text.Json;

namespace SerilogBlazor.Postgres;

public class SerilogPostgresQuery(
	string timezone,
	ILogger<SerilogPostgresQuery> logger,
	LoggingRequestIdProvider requestIdProvider,
	string connectionString, string schemaName = "public", string tableName = "serilog") : SerilogQuery
{
	private readonly string _timezone = timezone;
	private readonly ILogger<SerilogPostgresQuery> _logger = logger;
	private readonly LoggingRequestIdProvider _requestIdProvider = requestIdProvider;
	private readonly string _connectionString = connectionString;
	private readonly string _schemaName = schemaName;
	private readonly string _tableName = tableName;

	private class SerilogPostgresEntry
	{
		public int Id { get; init; }
		public DateTime Timestamp { get; init; }
		public int AgeMinutes { get; init; } // calculated as EXTRACT(EPOCH FROM (now() - timestamp))/60
		public string? SourceContext { get; init; }
		public string? RequestId { get; init; }
		public string Level { get; init; } = default!;
		public string MessageTemplate { get; init; } = default!;
		public string Message { get; init; } = default!;
		public string? UserName { get; init; }
		public string? Exception { get; init; }
		public string? PropertyJson { get; init; } // Postgres stores properties as JSON
	}

	protected override async Task<IEnumerable<SerilogEntry>> ExecuteInternalAsync(Criteria? criteria = null, int offset = 0, int limit = 50)
	{
		using var cn = new NpgsqlConnection(_connectionString);
		await cn.OpenAsync();

		var (whereClause, parameters, _) = GetWhereClause(criteria, _timezone);

		var query = 
			@$"SELECT 
				""id"" AS ""Id"", ""timestamp"" AS ""Timestamp"", 
				EXTRACT(EPOCH FROM (NOW() AT TIME ZONE '{_timezone}' - ""timestamp""))/60 AS ""AgeMinutes"", 
				""source_context"" AS ""SourceContext"", ""request_id"" AS ""RequestId"", ""level"" AS ""Level"", ""message_template"" AS ""MessageTemplate"", 
				""message"" AS ""Message"", ""exception"" AS ""Exception"", ""properties"" AS ""PropertyJson"", ""user_name"" AS ""UserName""
			FROM ""{_schemaName}"".""{_tableName}""
			{whereClause} 
			ORDER BY ""timestamp"" DESC 
			LIMIT {limit} OFFSET {offset}";

		_logger.BeginRequestId(_requestIdProvider.NextId());

		_logger.LogDebug("Querying serilog: {query}", query);

		var sw = Stopwatch.StartNew();
		bool error = false;
		try
		{
			var results = await cn.QueryAsync<SerilogPostgresEntry>(query, parameters);

			// Property values are now handled in the WHERE clause via jsonb containment operator
			// todo: apply non-query criteria (e.g., HasProperties if needed)

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

	private SerilogEntry ToSerilogEntry(SerilogPostgresEntry source) => new()
	{
		Id = source.Id,
		Timestamp = source.Timestamp,
		AgeText = DateHelper.ParseAgeText(source.AgeMinutes),
		SourceContext = source.SourceContext,
		RequestId = source.RequestId,
		Level = source.Level,
		MessageTemplate = source.MessageTemplate,
		Message = source.Message,
		Exception = source.Exception,
		UserName = source.UserName,
		Properties = ParseJsonProperties(source.PropertyJson)
	};

	private static Dictionary<string, object> ParseJsonProperties(string? propertyJson)
	{
		if (string.IsNullOrEmpty(propertyJson))
			return [];

		try
		{
			var jsonDoc = JsonDocument.Parse(propertyJson);
			var result = new Dictionary<string, object>();

			foreach (var property in jsonDoc.RootElement.EnumerateObject())
			{
				result[property.Name] = property.Value.ValueKind switch
				{
					JsonValueKind.String => property.Value.GetString() ?? string.Empty,
					JsonValueKind.Number => property.Value.GetDecimal(),
					JsonValueKind.True => true,
					JsonValueKind.False => false,
					JsonValueKind.Null => "null",
					_ => property.Value.ToString()
				};
			}

			return result;
		}
		catch (JsonException)
		{
			// If JSON parsing fails, return empty dictionary
			return [];
		}
	}

	private static (string, DynamicParameters? Parameters, IEnumerable<string> SearchTerms) GetWhereClause(Criteria? criteria, string timezone)
	{
		if (criteria is null) return (string.Empty, null, []);

		List<(string Sql, string Display)> terms = [];
		var parameters = new DynamicParameters();

		if (criteria.FromTimestamp.HasValue)
		{
			parameters.Add("@fromTimestamp", criteria.FromTimestamp.Value);
			terms.Add(($"\"Timestamp\">=@fromTimestamp", $"Timestamp after {criteria.FromTimestamp}"));
		}

		if (criteria.ToTimestamp.HasValue)
		{
			parameters.Add("@toTimestamp", criteria.ToTimestamp.Value);
			terms.Add(($"\"Timestamp\"<=@toTimestamp", $"Timestamp before {criteria.ToTimestamp}"));
		}

		if (criteria.Age.HasValue)
		{
			var ts = criteria.Age.Value;
			string intervalExpression;
			string displayText;

			// Convert TimeSpan to PostgreSQL interval
			if (ts.Days >= 30 && ts.Days % 30 == 0)
			{
				var months = ts.Days / 30;
				intervalExpression = $"INTERVAL '{months} months'";
				displayText = $"At most {months} months ago";
			}
			else if (ts.Days >= 7 && ts.Days % 7 == 0)
			{
				var weeks = ts.Days / 7;
				intervalExpression = $"INTERVAL '{weeks} weeks'";
				displayText = $"At most {weeks} weeks ago";
			}
			else if (ts.Days > 0)
			{
				intervalExpression = $"INTERVAL '{ts.Days} days'";
				displayText = $"At most {ts.Days} days ago";
			}
			else if (ts.Hours > 0)
			{
				intervalExpression = $"INTERVAL '{ts.Hours} hours'";
				displayText = $"At most {ts.Hours} hours ago";
			}
			else if (ts.Minutes > 0)
			{
				intervalExpression = $"INTERVAL '{ts.Minutes} minutes'";
				displayText = $"At most {ts.Minutes} minutes ago";
			}
			else if (ts.Seconds > 0)
			{
				intervalExpression = $"INTERVAL '{ts.Seconds} seconds'";
				displayText = $"At most {ts.Seconds} seconds ago";
			}
			else
			{
				throw new ArgumentException("Unsupported age format");
			}

			var ageDiff = $"NOW() AT TIME ZONE '{timezone}' - \"timestamp\"";
			terms.Add(($"{ageDiff} <= {intervalExpression}", displayText));
		}

		if (!string.IsNullOrEmpty(criteria.SourceContext))
		{
			parameters.Add("@sourceContext", $"%{criteria.SourceContext}%");
			terms.Add(($"\"source_context\" ILIKE @sourceContext", $"Source context contains '{criteria.SourceContext}'"));
		}

		if (!string.IsNullOrEmpty(criteria.RequestId))
		{
			parameters.Add("@requestId", criteria.RequestId);
			terms.Add(($"\"request_id\"=@requestId", $"Request Id = {criteria.RequestId}"));
		}

		if (!string.IsNullOrEmpty(criteria.Level))
		{			
			parameters.Add("@level", criteria.Level);
			terms.Add(($"\"level\"=@level", $"Level = {criteria.Level}"));
		}

		if (!string.IsNullOrEmpty(criteria.Message))
		{
			parameters.Add("@message", $"%{criteria.Message}%");
			terms.Add(($"\"message\" ILIKE @message", $"Message contains '{criteria.Message}'"));
		}

		if (!string.IsNullOrEmpty(criteria.Exception))
		{
			parameters.Add("@exception", $"%{criteria.Exception}%");
			terms.Add(($"\"exception\" ILIKE @exception", $"Exception contains '{criteria.Exception}'"));
		}

		// Handle property values using jsonb containment operator
		foreach (var propertyValue in criteria.HasPropertyValues)
		{
			// Build jsonb containment check
			// For properties stored as: {"Properties": {"apptId": 64696}}
			// We need: (properties -> 'Properties') @> '{"apptId": 64696}'
			
			string jsonValue;
			if (propertyValue.Value is string strValue)
			{
				// Escape backslashes first, then double quotes for JSON
				var escapedValue = strValue.Replace("\\", "\\\\").Replace("\"", "\\\"");
				jsonValue = $"\"{escapedValue}\"";
			}
			else if (propertyValue.Value is int or long or short or byte)
			{
				// Integer types don't need culture-specific formatting
				jsonValue = propertyValue.Value.ToString()!;
			}
			else if (propertyValue.Value is decimal or float or double)
			{
				// Use invariant culture for decimal formatting to ensure period as decimal separator
				jsonValue = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}", propertyValue.Value);
			}
			else
			{
				// Fallback for other types, use invariant culture
				jsonValue = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}", propertyValue.Value);
			}
			
			// Property key is restricted to \w+ by the regex, so it's safe to use directly
			var jsonbCheck = $@"(""properties"" -> 'Properties') @> '{{""{propertyValue.Key}"": {jsonValue}}}'";
			terms.Add((jsonbCheck, $"Property '{propertyValue.Key}' = {propertyValue.Value}"));
		}

		return 
			(terms.Any() ? $"WHERE {string.Join(" AND ", terms.Select(item => item.Sql))}" : string.Empty, 
			parameters, 
			terms.Select(item => item.Display));
	}

	public override IEnumerable<string> GetSearchTerms(Criteria criteria)
	{
		var (_, _, searchTerms) = GetWhereClause(criteria, _timezone);
		return searchTerms;
	}
}