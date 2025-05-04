using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.IndexedLogContext;

namespace Service;

/// <summary>
/// queries the serilog table for new logs and extracts and transforms select info
/// </summary>
public class SerilogIndexer<TDbContext>(
	IDbContextFactory<TDbContext> dbFactory,
	ILogger<SerilogIndexer<TDbContext>> logger
	) : IInvocable where TDbContext : DbContext, IIndexedLogContext
{
	private readonly IDbContextFactory<TDbContext> _dbFactory = dbFactory;
	private readonly ILogger<SerilogIndexer<TDbContext>> _logger = logger;

	public async Task Invoke()
	{
		_logger.LogDebug("SerilogIndexer invoked");

		using var db = _dbFactory.CreateDbContext();

		var serilogTableMarkers = await db.SerilogTableMarkers
			.AsNoTracking()
			.ToArrayAsync();
	}
}
