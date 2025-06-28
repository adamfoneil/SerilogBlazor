using Microsoft.EntityFrameworkCore;

namespace SerilogViewer.Abstractions.SourceContextView;

public interface ISourceContextViewState
{
	DbSet<SourceContextView> SourceContexts { get; set; }

	Task<SourceContextViewItem[]> GetSourceContextViewItemsAsync(string userName);

	Task SetSourceContextVisibilityAsync(string userName, string sourceContext, string level, bool isVisible);
}

public class SourceContextViewItem
{
	public string SourceContext { get; set; } = default!;
	public string Level { get; set; } = default!;
	public DateTime LatestTimestamp { get; set; }
	public string AgeText { get; set; } = default!;
	public bool IsVisible { get; set; }
}