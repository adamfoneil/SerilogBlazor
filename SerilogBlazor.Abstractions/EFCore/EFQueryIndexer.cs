using Coravel.Invocable;
using Microsoft.Extensions.Logging;

namespace SerilogBlazor.Abstractions.EFCore;

public class SerilogEFEntry
{
    /// <summary>
    /// FK to original Serilog log entry, should cascade delete
    /// </summary>
    public int SourceLogId { get; init; }
    public DateTime Timestamp { get; init; }
    public string? AgeText { get; init; } = default!;
    /// <summary>
    /// select, insert, update, delete
    /// </summary>
    public string Action { get; init; } = default!;
    /// <summary>
    /// covered object names (tables, views, functions)
    /// </summary>
    public string[] ObjectNames { get; init; } = [];
    /// <summary>
    /// unescaped raw SQL from source log
    /// </summary>
    public string SQL { get; init; } = default!;
    /// <summary>
    /// param names and values
    /// </summary>
    public string[] Parameters { get; init; } = [];
    /// <summary>
    /// TagWith values from C# code
    /// </summary>
    public string[] Tags { get; init; } = [];
    public long ElapsedMS { get; init; }
}

public abstract class EFQueryIndexer(ILogger<EFQueryIndexer> logger) : IInvocable
{
    protected ILogger<EFQueryIndexer> Logger { get; } = logger;

    /// <summary>
    /// get logs since the last indexing operation (where source context = Microsoft.EntityFrameworkCore.Database.Command)
    /// </summary>
    protected abstract Task<SerilogEntry[]> QueryLatestLogsAsync();

    /// <summary>
    /// how do we extract relevant info from the log entry?
    /// </summary>
    protected abstract SerilogEFEntry? ParseEFCoreQuery(SerilogEntry logEntry);

    /// <summary>
    /// store the parsed query logs
    /// </summary>
    protected abstract Task SaveQueryLogsAsync(IEnumerable<SerilogEFEntry> queryLogs);

    public async Task Invoke()
    {
        try
        {
            var logs = await QueryLatestLogsAsync();
            if (!logs.Any())
            {
                Logger.LogDebug("EFQueryIndexer found no new logs to process");
                return;
            }

            var parsed = logs.Select(ParseEFCoreQuery).Where(x => x is not null) ?? [];
            if (!parsed.Any())
            {
                Logger.LogInformation("EFQueryIndexer found no EF Core queries to index");
                return;
            }

            await SaveQueryLogsAsync(parsed!);
        }
        catch (Exception exc)
        {
            Logger.LogError(exc, "EFQueryIndexer failed");
        }
    }
}
