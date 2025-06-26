namespace SerilogViewer.Abstractions;

/// <summary>
/// Blazor apps can't make use of the RequestId from ASP.NET Core, 
/// so this provides a way to generate an incrementing request id for a logging scope.
/// Also works for anything outside of a web request, e.g. background tasks.
/// Add as singleton to your services collection.
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
		return $"{_date:yyyyMMdd}-{id:0000000}";
	}
}
