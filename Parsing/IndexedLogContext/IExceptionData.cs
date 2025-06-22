namespace Service.IndexedLogContext;

public interface IExceptionData
{
	int Id { get; set; }
	string SourceContext { get; set; }
	DateTime Timestamp { get; set; }
	string Message { get; set; }
	string StackTrace { get; set; }
}
