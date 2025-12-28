using Microsoft.EntityFrameworkCore;
using SampleApp.Data;

namespace SampleApp;

public class SampleService(ILogger<SampleService> logger, IDbContextFactory<ApplicationDbContext> contextFactory)
{
	private readonly ILogger<SampleService> _logger = logger;
	private readonly IDbContextFactory<ApplicationDbContext> _contextFactory = contextFactory;

	public void DoWork()
	{
		_logger.LogDebug("Something low-level with short term value");
		_logger.LogInformation("Doing work at {time}", DateTimeOffset.Now);
	}

	public async Task DoEfCoreQuery()
	{
		using var context = _contextFactory.CreateDbContext();
		
		// Enable sensitive data logging and detailed errors for demonstration
		context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
		
		// Perform a simple query that will generate EF Core logs
		var count = await context.ExceptionTemplates.CountAsync();
		_logger.LogInformation("Found {count} exception templates", count);
		
		// Perform a more complex query with parameters
		var recentTemplates = await context.ExceptionTemplates
			.Where(t => t.Id > 0)
			.Take(5)
			.ToListAsync();
		
		_logger.LogInformation("Retrieved {count} recent templates", recentTemplates.Count);
	}
}
