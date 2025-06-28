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
		public string? SourceContext { get; set; }
		public string? RequestId { get; set; }
		public string? Level { get; set; }
		public string? Message { get; set; }
		public string? Exception { get; set; }
		public HashSet<string> HasProperties { get; set; } = [];
		public Dictionary<string, object> HassPropertyValues { get; set; } = [];
	}	
}
