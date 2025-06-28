using Microsoft.EntityFrameworkCore;

namespace SerilogViewer.Abstractions.SourceContextView;

public interface ISourceContextViewState
{
	DbSet<SourceContextView> SourceContexts { get; set; }
}

public class SourceContextViewItem
{
	public string SourceContext { get; set; } = default!;
	public string Level { get; set; } = default!;
	public DateTime LatestTimestamp { get; set; }
	public string AgeText { get; set; } = default!;
	public int Count { get; set; }
	public bool IsVisible { get; set; }
}