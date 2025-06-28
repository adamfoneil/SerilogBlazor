using Microsoft.EntityFrameworkCore;
using SerilogViewer.Abstractions;
using SerilogViewer.Abstractions.SourceContextView;

namespace SerilogViewer.SqlServer;

public static class SerilogSourceContextViewState
{
	public static async Task<SourceContextViewItem[]> QuerySourceContextViewItemsAsync<TDbContext>(
		this TDbContext dbContext, string userName) where TDbContext : ISourceContextViewState
	{
		if (dbContext is not DbContext context)
			throw new ArgumentException("DbContext must derive from EntityFramework DbContext", nameof(dbContext));

		var currentTime = DateTime.UtcNow;

		// Use EF Core LINQ query to get source context summary data
		var summaryData = await dbContext.SourceContexts
			.GroupBy(entry => new { entry.SourceContext, entry.Level })
			.Select(group => new
			{
				group.Key.SourceContext,
				group.Key.Level,
				LatestTimestamp = group.Max(e => e.Timestamp),
				Count = group.Count()
			})
			.OrderByDescending(item => item.LatestTimestamp)
			.ToArrayAsync();

		// Get visibility settings for the user
		var visibilitySettings = await dbContext.SourceContexts
			.Where(sc => sc.UserName == userName)
			.ToArrayAsync();

		// Combine the data and calculate AgeText
		var result = summaryData.Select(item => new SourceContextViewItem
		{
			SourceContext = item.SourceContext ?? "",
			Level = item.Level,
			LatestTimestamp = item.LatestTimestamp,
			Count = item.Count,
			AgeText = DateHelper.ParseAgeText((int)(currentTime - item.LatestTimestamp).TotalMinutes),
			IsVisible = visibilitySettings
				.FirstOrDefault(vs => vs.SourceContext == item.SourceContext && vs.Level == item.Level)
				?.IsVisible ?? true // Default to visible if no setting exists
		}).ToArray();

		return result;
	}

	public static async Task SetSourceContextVisibilityAsync(
		this ISourceContextViewState sourceContextViewState, string userName, string sourceContext, string level, bool isVisible)
	{
		await sourceContextViewState.SetSourceContextVisibilityAsync(userName, sourceContext, level, isVisible);
	}
}
