namespace SampleApp;

public class SampleService(ILogger<SampleService> logger)
{
	private readonly ILogger<SampleService> _logger = logger;

	public void DoWork()
	{
		_logger.LogDebug("Something low-level with short term value");
		_logger.LogInformation("Doing work at {time}", DateTimeOffset.Now);
	}
}
