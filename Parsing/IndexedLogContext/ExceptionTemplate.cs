namespace Service.IndexedLogContext;

public class ExceptionTemplate
{
	public int Id { get; set; }
	/// <summary>
	/// md5 hash of error location
	/// </summary>
	public string ErrorId { get; set; } = default!;
	/// <summary>
	/// exception type
	/// </summary>
	public string Type { get; set; } = default!;
	/// <summary>
	/// first message of this ErrorId
	/// </summary>
	public string Message { get; set; } = default!;
	/// <summary>
	/// json parse of stack trace
	/// </summary>
	public string Data { get; set; } = default!;
}
