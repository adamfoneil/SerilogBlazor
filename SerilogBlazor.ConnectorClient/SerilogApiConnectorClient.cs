using SerilogBlazor.Abstractions;

namespace SerilogBlazor.ConnectorClient;

public class SerilogApiConnectorClient(IHttpClientFactory httpClientFactory)
{
	private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

	public async Task<SerilogEntry[]> GetEntriesAsync(string endpoint, string headerSecret, string? criteria = null)
	{
		throw new NotImplementedException();
	}

	public async Task<SourceContextMetricsResult[]> GetMetricsAsync(string endpoint, string headerSecret)
	{
		throw new NotImplementedException();
	}

	// todo: log levels
}
