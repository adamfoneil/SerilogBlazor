using SerilogBlazor.Abstractions;

namespace SerilogBlazor.Postgres;

public static class PostgresHelpers
{
	public static string CurrentTimeFunction(TimestampType timestampType) => timestampType switch
	{
		TimestampType.Utc => "NOW() AT TIME ZONE 'UTC'",
		TimestampType.Local => "NOW()",
		_ => throw new ArgumentOutOfRangeException(nameof(timestampType))
	};

	/// <summary>
	/// Converts Serilog level integer to string for UI display
	/// </summary>
	public static string LevelIntToString(int levelValue) => levelValue switch
	{
		0 => "Verbose",
		1 => "Debug", 
		2 => "Information",
		3 => "Warning",
		4 => "Error",
		5 => "Fatal",
		_ => levelValue.ToString()
	};

	/// <summary>
	/// Converts Serilog level string to integer for queries
	/// </summary>
	public static int LevelStringToInt(string levelString) => levelString switch
	{
		"Verbose" => 0,
		"Debug" => 1,
		"Information" => 2,
		"Warning" => 3,
		"Error" => 4,
		"Fatal" => 5,
		_ => throw new ArgumentException($"Unknown level: {levelString}", nameof(levelString))
	};
}