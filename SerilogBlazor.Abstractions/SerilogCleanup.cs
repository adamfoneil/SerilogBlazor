using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.Diagnostics;

namespace SerilogBlazor.Abstractions;

public class SerilogCleanupOptions
{
	public required string ConnectionString { get; init; }
	public required string TableName { get; init; }
	/// <summary>
	/// Debug level retention in days
	/// </summary>
	public int Debug { get; set; } = 5;
	/// <summary>
	/// Information level retention in days
	/// </summary>
	public int Information { get; set; } = 60;
	/// <summary>
	/// Warning level retention in days
	/// </summary>
	public int Warning { get; set; } = 10;
	/// <summary>
	/// Error level retention in days
	/// </summary>
	public int Error { get; set; } = 30;

	public Dictionary<string, int> RetentionDays => new()
	{
		[nameof(Debug)] = Debug,
		[nameof(Information)] = Information,
		[nameof(Warning)] = Warning,
		[nameof(Error)] = Error
	};
}

/// <summary>
/// add as singleton to DI to periodically clean up old serilog entries
/// </summary>
public abstract class SerilogCleanup(
	LoggingRequestIdProvider requestIdProvider,
	ILogger<SerilogCleanup> logger,
	IOptions<SerilogCleanupOptions> options) : IInvocable
{
	protected readonly SerilogCleanupOptions Options = options.Value;
	protected readonly ILogger<SerilogCleanup> Logger = logger;

	private readonly LoggingRequestIdProvider _requestIdProvider = requestIdProvider;
	
	public async Task ExecuteAsync()
	{
		using var cn = GetConnection();
		cn.Open();

		Logger.BeginRequestId(_requestIdProvider);

		foreach (var (logLevel, retentionDays) in Options.RetentionDays)
		{
			var sw = Stopwatch.StartNew();
			bool error = false;
			try
			{
				var deleted = await DeleteOldEntriesAsync(cn, logLevel, retentionDays);				
				Logger.LogInformation("Deleted {deleted} {logLevel} entries older than {retentionDays} days", deleted, logLevel, retentionDays);
			}
			catch (Exception exc)
			{
				error = true;
				Logger.LogError(exc, "Error deleting old {logLevel} entries older than {retentionDays} days", logLevel, retentionDays);
			}
			finally
			{
				Logger.LogInformation("Serilog cleanup for {logLevel} entries {status} in {elapsed} ms", logLevel, error ? "failed" : "succeeded", sw.ElapsedMilliseconds);
			}
		}
	}

	public async Task Invoke()
	{
		Logger.LogDebug("Running scheduled Serilog cleanup");
		await ExecuteAsync();
	}

	protected abstract IDbConnection GetConnection();

	protected abstract Task<int> DeleteOldEntriesAsync(IDbConnection cn, string logLevel, int retentionDays);
}
