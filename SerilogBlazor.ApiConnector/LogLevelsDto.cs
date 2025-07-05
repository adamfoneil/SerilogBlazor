using System.ComponentModel.DataAnnotations;

namespace SerilogBlazor.ApiConnector;

/// <summary>
/// Data transfer object for log levels configuration
/// </summary>
public class LogLevelsDto
{
	/// <summary>
	/// The default minimum log level
	/// </summary>
	[Required]
	public string DefaultLevel { get; set; } = default!;

	/// <summary>
	/// Configured log levels for specific namespaces
	/// </summary>
	[Required]
	public Dictionary<string, string> ConfiguredLevels { get; set; } = new();
}