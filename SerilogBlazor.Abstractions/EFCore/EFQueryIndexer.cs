using Coravel.Invocable;
using Microsoft.Extensions.Logging;

namespace SerilogBlazor.Abstractions.EFCore;

public class SerilogEFEntry
{
    /// <summary>
    /// FK to original Serilog log entry, should cascade delete
    /// </summary>
    public int SourceLogId { get; set; }
    public DateTime Timestamp { get; set; }
    public string? AgeText { get; set; } = default!;
    /// <summary>
    /// select, insert, update, delete
    /// </summary>
    public string Action { get; set; } = default!;
    /// <summary>
    /// covered object names (tables, views, functions)
    /// </summary>
    public string[] ObjectNames { get; set; } = [];
    /// <summary>
    /// unescaped raw SQL from source log
    /// </summary>
    public string SQL { get; set; } = default!;
    /// <summary>
    /// param names and values
    /// </summary>
    public string[] Parameters { get; set; } = [];
    /// <summary>
    /// TagWith values from C# code
    /// </summary>
    public string[] Tags { get; set; } = [];
    public long ElapsedMS { get; set; }
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
