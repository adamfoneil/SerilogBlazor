using SerilogBlazor.Abstractions;

namespace SerilogBlazor.ConnectorClient;

public class SerilogApiConnectorClient(IHttpClientFactory httpClientFactory)
{
	private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

	public async Task<SerilogEntry[]> QueryEntriesAsync(string endpoint, string headerSecret, string? criteria = null)
	{
		throw new NotImplementedException();
	}

	public async Task<SourceContextMetricsResult[]> QueryMetricsAsync(string endpoint, string headerSecret)
	{
		throw new NotImplementedException();
	}

	// todo: log levels
}
