namespace SerilogBlazor.Abstractions;

public class SerilogEntry
{
	public int Id { get; init; }
	public DateTime Timestamp { get; init; }
	public string? AgeText { get; init; } = default!;
	public string? SourceContext { get; init; }
	public string? RequestId { get; init; }
	public string Level { get; init; } = default!;
	public string MessageTemplate { get; init; } = default!;
	public string Message { get; init; } = default!;
	public string? Exception { get; init; }
	public string? UserName { get; init; }
	public Dictionary<string, object> Properties { get; init; } = [];
}	

public enum TimestampType
{
	Local,
	Utc
}

public abstract class SerilogQuery(TimestampType timestampType = TimestampType.Utc)
{
	protected readonly TimestampType TimestampType = timestampType;

	public abstract Task<IEnumerable<SerilogEntry>> ExecuteAsync(Criteria? criteria = null, int offset = 0, int limit = 50);

	protected abstract IEnumerable<string> GetSearchTerms(Criteria criteria);

	public (Criteria Criteria, string[] SearchTerms) ParseCriteria(string input)
	{
		var criteria = Criteria.ParseExpression(input);
		return (criteria, [.. GetSearchTerms(criteria)]);
	}

	public class Criteria
	{
		public DateTime? FromTimestamp { get; set; }
		public DateTime? ToTimestamp { get; set; }
		public TimeSpan? Age { get; set; }
		public string? SourceContext { get; set; }
		public string? RequestId { get; set; }
		public string? Level { get; set; }
		public string? Message { get; set; }
		public string? Exception { get; set; }
		public HashSet<string> HasProperties { get; set; } = [];
		public Dictionary<string, object> HassPropertyValues { get; set; } = [];

		public static Criteria ParseExpression(string input)
		{
			/*
			 [{string}] = SourceContext (assume wildcards around it)
			 @{string} = Level (can be partial like "warn" "info" or "err")
			 #{string} = RequestId
			 -{string} = age expression (e.g., -7d for up to 7 days ago, or -30m for up to 30 minutes ago), supported types: d, h, hr, m, s, wk, mon
			!{string} = exception text
			{string} = anything not punctuated is assumed to be a message search term
			*/

			if (string.IsNullOrWhiteSpace(input))
				return new Criteria();

			var criteria = new Criteria();			
			var remainingText = input.Trim();

			// Process tokens with special prefixes
			remainingText = ProcessSourceContext(remainingText, criteria);
			remainingText = ProcessLevel(remainingText, criteria);
			remainingText = ProcessRequestId(remainingText, criteria);
			remainingText = ProcessAge(remainingText, criteria);
			remainingText = ProcessException(remainingText, criteria);
			
			// Process quoted messages
			remainingText = ProcessQuotedMessage(remainingText, criteria);

			// Whatever remains is treated as message text
			if (!string.IsNullOrWhiteSpace(remainingText))
			{
				if (string.IsNullOrEmpty(criteria.Message))
					criteria.Message = remainingText.Trim();
				else
					criteria.Message = remainingText.Trim() + " " + criteria.Message;
			}

			return criteria;
		}

		private static string ProcessSourceContext(string input, Criteria criteria)
		{
			var regex = new System.Text.RegularExpressions.Regex(@"\[([^\]]+)\]");
			var match = regex.Match(input);
			if (match.Success)
			{
				criteria.SourceContext = match.Groups[1].Value;
				input = regex.Replace(input, "").Trim();
			}
			return input;
		}

		private static string ProcessLevel(string input, Criteria criteria)
		{
			var regex = new System.Text.RegularExpressions.Regex(@"@(\w+)");
			var match = regex.Match(input);
			if (match.Success)
			{
				var levelInput = match.Groups[1].Value.ToLower();
				criteria.Level = MapLevel(levelInput);
				input = regex.Replace(input, "").Trim();
			}
			return input;
		}

		private static string ProcessRequestId(string input, Criteria criteria)
		{
			var regex = new System.Text.RegularExpressions.Regex(@"#(\S+)");
			var match = regex.Match(input);
			if (match.Success)
			{
				criteria.RequestId = match.Groups[1].Value;
				input = regex.Replace(input, "").Trim();
			}
			return input;
		}

		private static string ProcessAge(string input, Criteria criteria)
		{
			var regex = new System.Text.RegularExpressions.Regex(@"-(\d+)(d|h|hr|m|s|wk|mon)\b");
			var match = regex.Match(input);
			if (match.Success)
			{
				var amount = int.Parse(match.Groups[1].Value);
				var unit = match.Groups[2].Value;
				
				criteria.Age = unit switch
				{
					"d" => TimeSpan.FromDays(amount),
					"h" => TimeSpan.FromHours(amount),
					"hr" => TimeSpan.FromHours(amount),
					"m" => TimeSpan.FromMinutes(amount),
					"s" => TimeSpan.FromSeconds(amount),
					"wk" => TimeSpan.FromDays(amount * 7),
					"mon" => TimeSpan.FromDays(amount * 30),
					_ => throw new ArgumentException($"Unknown time unit: {unit}")
				};
				
				input = regex.Replace(input, "").Trim();
			}
			return input;
		}

		private static string ProcessException(string input, Criteria criteria)
		{
			var regex = new System.Text.RegularExpressions.Regex(@"!(\w+)");
			var match = regex.Match(input);
			if (match.Success)
			{
				criteria.Exception = match.Groups[1].Value;
				input = regex.Replace(input, "").Trim();
			}
			return input;
		}

		private static string ProcessQuotedMessage(string input, Criteria criteria)
		{
			var regex = new System.Text.RegularExpressions.Regex(@"""([^""]+)""");
			var match = regex.Match(input);
			if (match.Success)
			{
				criteria.Message = match.Groups[1].Value;
				input = regex.Replace(input, "").Trim();
			}
			return input;
		}

		private static string MapLevel(string input)
		{
			return input.ToLower() switch
			{
				"err" or "error" => "Error",
				"warn" or "warning" => "Warning",
				"info" or "information" => "Info",
				"debug" => "Debug",
				"trace" => "Trace",
				"fatal" => "Fatal",
				_ => char.ToUpper(input[0]) + input[1..].ToLower()
			};
		}
	}	
}
