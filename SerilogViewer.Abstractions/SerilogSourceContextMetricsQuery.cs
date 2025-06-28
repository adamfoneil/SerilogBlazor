namespace SerilogViewer.Abstractions;

public class SourceContextMetricsResult
{
	public string SourceContext { get; set; } = default!;
	public string Level { get; set; } = default!;
	public DateTime LatestTimestamp { get; set; }
	public string AgeText { get; set; } = default!;
	public int Count { get; set; }
	public bool IsVisible { get; set; } = true;
}

public abstract class SerilogSourceContextMetricsQuery
{
	public abstract Task<IEnumerable<SourceContextMetricsResult>> ExecuteAsync();
}
