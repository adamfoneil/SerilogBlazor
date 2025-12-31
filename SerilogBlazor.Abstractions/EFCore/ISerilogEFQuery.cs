namespace SerilogBlazor.Abstractions.EFCore;

public interface ISerilogEFQuery
{
    Task<IEnumerable<SerilogEFEntry>> ExecuteAsync(
        string? action = null, string? objectName = null, string? sql = null, string? tag = null,
        int offset = 0, int limit = 50);
}
