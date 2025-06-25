namespace SerilogViewer.Abstractions;

public record StackTraceInfo(string ExceptionType, string Message, CodeLocation[] Locations)
{
	/// <summary>
	/// unique identifier for where this error is happening
	/// </summary>
	public string ErrorId => string.Join("|", Locations.Select(loc => $"{loc.MethodName}:{loc.LineNumber}")).ToMd5();
}

public record CodeLocation(string MethodName, string Filename, int LineNumber);
