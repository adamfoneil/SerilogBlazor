namespace SampleApp;

public class SampleService(ILogger<SampleService> logger)
{
	private readonly ILogger<SampleService> _logger = logger;

	public void DoWork()
	{
		_logger.LogInformation("Doing work at {time}", DateTimeOffset.Now);
	}
}
