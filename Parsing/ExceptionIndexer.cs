using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.IndexedLogContext;

namespace Service;

/// <summary>
/// queries the serilog table for new logs and extracts and transforms stack traces
/// </summary>
public abstract class ExceptionIndexer<TDbContext>(
	IDbContextFactory<TDbContext> dbFactory,
	ILogger<ExceptionIndexer<TDbContext>> logger
	) : IInvocable 
	where TDbContext : DbContext, IIndexedLogContext
{
	private readonly IDbContextFactory<TDbContext> _dbFactory = dbFactory;
	private readonly ILogger<ExceptionIndexer<TDbContext>> _logger = logger;

	public async Task Invoke()
	{
		_logger.LogDebug("SerilogIndexer invoked");

		using var db = _dbFactory.CreateDbContext();

		var serilogTableMarkers = await db
			.SerilogTableMarkers
			.AsNoTracking()
			.ToArrayAsync();

		foreach (var marker in serilogTableMarkers)
		{
			_logger.LogDebug("Processing marker {MarkerId}", marker.Id);

			var logs = await QueryExceptionsAsync(marker.LogId);
		}
	}

	protected abstract Task<IExceptionData[]> QueryExceptionsAsync(int marker);
}
