namespace SerilogViewer.Abstractions;

/// <summary>
/// Blazor apps can't make use of the RequestId from ASP.NET Core, 
/// so we need to provide a way to generate or retrieve a request ID for logging purposes.
/// </summary>
public class LoggingRequestIdProvider
{
	private int _currentId = 0;

	public string NextId()
	{
		var id = Interlocked.Increment(ref _currentId);
		return $"request-{id}";
	}
}
