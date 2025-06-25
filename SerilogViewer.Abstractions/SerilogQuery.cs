namespace SerilogViewer.Abstractions;

public record SerilogEntry(
	int Id,
	DateTime Timestamp,
	string? SourceContext,
	string? RequestId,
	string Level,
	string MessageTemplate,
	string Message,
	string? Exception,
	Dictionary<string, object> Properties);

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
