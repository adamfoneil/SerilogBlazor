namespace SerilogViewer.Abstractions;

public class SerilogEntry
{
	public int Id { get; init; }
	public DateTime Timestamp { get; init; }
	public string? SourceContext { get; init; }
	public string? RequestId { get; init; }
	public string Level { get; init; } = default!;
	public string MessageTemplate { get; init; } = default!;
	public string Message { get; init; } = default!;
	public string? Exception { get; init; }
	public Dictionary<string, object> Properties { get; init; } = [];
}	

public abstract class SerilogQuery
{
	public abstract Task<IEnumerable<SerilogEntry>> ExecuteAsync(Criteria? criteria = null, int offset = 0, int limit = 50);

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
			 -{string} = age expression (e.g., -7d for up to 7 days ago, or -30m for up to 30 minutes ago), supported types: d, h, hr, m
			!{string} = exception text
			{string} = anything not punctuated is assumed to be a message search term
			*/

			throw new NotImplementedException();
		}
	}	
}
