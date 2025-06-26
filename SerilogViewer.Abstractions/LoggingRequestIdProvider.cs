namespace SerilogViewer.Abstractions;

/// <summary>
/// Blazor apps can't make use of the RequestId from ASP.NET Core, 
/// so we need to provide a way to generate or retrieve a request ID for logging purposes.
/// </summary>
public class LoggingRequestIdProvider
{
	private int _currentId = 0;
	private DateOnly _date = DateOnly.FromDateTime(DateTime.UtcNow);

	public string NextId()
	{
		var today = DateOnly.FromDateTime(DateTime.UtcNow);
		if (today > _date)
		{
			_date = today;
			_currentId = 0;
		}

		var id = Interlocked.Increment(ref _currentId);
		return $"{_date:yyyMMdd}-{id:0000000}";
	}
}
