using SerilogBlazor.Abstractions;

namespace SerilogBlazor.SqlServer;

internal static class SqlServerHelpers
{
	internal static string CurrentTimeFunction(TimestampType timestampType) => timestampType switch
	{
		TimestampType.Utc => "GETUTCDATE()",
		TimestampType.Local => "GETDATE()",
		_ => throw new ArgumentOutOfRangeException(nameof(timestampType))
	};
}
