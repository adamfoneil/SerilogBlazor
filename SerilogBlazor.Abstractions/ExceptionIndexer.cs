using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SerilogBlazor.Abstractions;
using SerilogBlazor.Abstractions.IndexedLogContext;

namespace SerilogBlazor.Service;

public class ExceptionIndexerOptions
{
	/// <summary>
	/// portion of source file path to remove from stack traces (typically repo base path)
	/// </summary>
	public string SourceBasePath { get; set; } = default!;
	/// <summary>
	/// namespace prefix that denotes code you wrote that could be associated with a stack trace
	/// </summary>
	public string MyCodePrefix { get; set; } = default!;
}

/// <summary>
/// queries the serilog table for new logs and extracts and transforms stack traces,
/// add as singleton to DI to periodically index exceptions
/// </summary>
public abstract class ExceptionIndexer<TDbContext>(
	IDbContextFactory<TDbContext> dbFactory,
	IOptions<ExceptionIndexerOptions> options,
	ILogger<ExceptionIndexer<TDbContext>> logger) : IInvocable 
	where TDbContext : DbContext, IIndexedLogContext
{
	private readonly IDbContextFactory<TDbContext> _dbFactory = dbFactory;
	private readonly ExceptionIndexerOptions _options = options.Value;
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

			try
			{
				_logger.LogDebug("Querying exceptions");
				var (maxId, logs) = await QueryExceptionsAsync(marker.LogId);

				foreach (var entry in logs)
				{
					var (success, info) = TryParseException(entry);
					
					if (success)
					{
						ArgumentNullException.ThrowIfNull(info, nameof(info));

						var exceptionTemplate = 
							await db.ExceptionTemplates.SingleOrDefaultAsync(row => row.ErrorId == info.ErrorId) ?? 
							new() 
							{ 
								ErrorId = info.ErrorId, 
								Message = entry.Message, 
								StackTraceData = entry.StackTrace,
								SourceContext = entry.SourceContext
							};

						exceptionTemplate.Instances.Add(new()
						{
							Timestamp = entry.Timestamp,
							Message = entry.Message,
							LogId = entry.Id
						});

						db.ExceptionTemplates.Update(exceptionTemplate);
					}
				}

				marker.LogId = maxId;
				db.SerilogTableMarkers.Update(marker);
			}
			catch (Exception exc)
			{
				_logger.LogError(exc, "Error processing marker {MarkerId}", marker.Id);
			}			
		}

		await db.SaveChangesAsync();
	}

	private (bool Success, StackTraceInfo? Info) TryParseException(IExceptionData entry)
	{
		try
		{
			var info = StackTraceParser.Parse(entry.StackTrace, _options.SourceBasePath, _options.MyCodePrefix);			
			return (true, info);
		}
		catch (Exception exc)
		{
			_logger.LogError(exc, "Error parsing exception {Id}", entry.Id);
			return (false, default);
		}
	}

	protected abstract Task<(int MaxLogId, IExceptionData[])> QueryExceptionsAsync(int fromLogId);
}
