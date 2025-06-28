using SerilogViewer.Abstractions.SourceContextView;

namespace SerilogViewer.SqlServer;

public static class SerilogSourceContextViewState
{
	public static async Task<SourceContextViewItem[]> QuerySourceContextViewItemsAsync<TDbContext>(
		this TDbContext dbContext, string userName) where TDbContext : ISourceContextViewState
	{
		throw new NotImplementedException();
	}

	public static async Task SetSourceContextVisibilityAsync(
		this ISourceContextViewState sourceContextViewState, string userName, string sourceContext, string level, bool isVisible)
	{
		await sourceContextViewState.SetSourceContextVisibilityAsync(userName, sourceContext, level, isVisible);
	}
}
