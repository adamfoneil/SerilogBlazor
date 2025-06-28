using System.Data.Common;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SerilogViewer.Abstractions.SourceContextView;

namespace SerilogViewer.SqlServer;

public static class SerilogSourceContextViewState
{
	public static async Task<SourceContextViewItem[]> QuerySourceContextViewItemsAsync<TDbContext>(
		this TDbContext dbContext, string userName) where TDbContext : ISourceContextViewState
	{
		if (dbContext is not DbContext context)
			throw new ArgumentException("DbContext must derive from EntityFramework DbContext", nameof(dbContext));

		// Get the underlying database connection
		var connection = context.Database.GetDbConnection();
		
		// Execute the T-SQL query to get source context summary data
		var query = @"
			SELECT 
				[SourceContext], 
				[Level], 
				MAX([Timestamp]) AS [LatestTimestamp], 
				COUNT(1) AS [Count],
				DATEDIFF(minute, MAX([Timestamp]), GETUTCDATE()) AS [AgeMinutes]
			FROM [log].[Serilog]
			GROUP BY [SourceContext], [Level]
			ORDER BY MAX([Timestamp]) DESC";

		// Ensure connection is open
		var shouldCloseConnection = connection.State != System.Data.ConnectionState.Open;
		if (shouldCloseConnection)
		{
			await connection.OpenAsync();
		}

		try
		{
			var summaryData = await connection.QueryAsync<SourceContextSummary>(query);

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
				AgeText = SerilogSqlServerQuery.ParseAgeText(item.AgeMinutes),
				IsVisible = visibilitySettings
					.FirstOrDefault(vs => vs.SourceContext == item.SourceContext && vs.Level == item.Level)
					?.IsVisible ?? true // Default to visible if no setting exists
			}).ToArray();

			return result;
		}
		finally
		{
			if (shouldCloseConnection)
			{
				await connection.CloseAsync();
			}
		}
	}

	// Internal class to hold the raw query results
	private class SourceContextSummary
	{
		public string? SourceContext { get; set; }
		public string Level { get; set; } = default!;
		public DateTime LatestTimestamp { get; set; }
		public int Count { get; set; }
		public int AgeMinutes { get; set; }
	}

	public static async Task SetSourceContextVisibilityAsync(
		this ISourceContextViewState sourceContextViewState, string userName, string sourceContext, string level, bool isVisible)
	{
		await sourceContextViewState.SetSourceContextVisibilityAsync(userName, sourceContext, level, isVisible);
	}
}
