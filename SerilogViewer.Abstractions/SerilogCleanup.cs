using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;

namespace SerilogViewer.Abstractions;

public class SerilogCleanupOptions
{
	public string ConnectionString { get; set; } = default!;	
	public int Debug { get; set; } = 3;
	public int Information { get; set; } = 30;
	public int Warning { get; set; } = 30;
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
	ILogger<SerilogCleanup> logger,
	IOptions<SerilogCleanupOptions> options) : IInvocable
{
	private readonly SerilogCleanupOptions _options = options.Value;
	private readonly ILogger<SerilogCleanup> _logger = logger;

	public async Task Invoke()
	{
		using var cn = GetConnection();
	}

	protected abstract IDbConnection GetConnection();

	protected abstract Task<int> DeleteOldEntriesAsync(IDbConnection cn, string logLevel, int retentionDays);
}
