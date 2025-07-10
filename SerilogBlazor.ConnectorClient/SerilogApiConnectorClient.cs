using Microsoft.Extensions.Logging;
using SerilogBlazor.Abstractions;
using System.Text.Json;

namespace SerilogBlazor.ConnectorClient;

public class SerilogApiConnectorClient(
	ILogger<SerilogApiConnectorClient> logger,
	IHttpClientFactory httpClientFactory, 
	string endpoint, string headerSecret) : ISerilogQuery
{
	private readonly ILogger<SerilogApiConnectorClient> _logger = logger;
	private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
	private readonly string _endpoint = endpoint;
	private readonly string _headerSecret = headerSecret;

	private HttpClient InitClient()
	{
		_logger.LogDebug("Initializing Serilog API client with endpoint: {endpoint}, header secret {secret}", _endpoint, _headerSecret);
		var client = _httpClientFactory.CreateClient();
		client.BaseAddress = new Uri(_endpoint.TrimEnd('/') + "/");
		client.DefaultRequestHeaders.Add("serilog-api-secret", _headerSecret);
		return client;
	}

	public async Task<IEnumerable<SerilogEntry>> ExecuteAsync(string? query = null, int offset = 0, int limit = 50)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(_endpoint);
		ArgumentException.ThrowIfNullOrWhiteSpace(_headerSecret);

		using var httpClient = InitClient();
		
		var requestUri = "detail";
		var queryParams = new List<string>();

		if (!string.IsNullOrWhiteSpace(query))
		{
			var encodedquery = Uri.EscapeDataString(query);
			queryParams.Add($"search={encodedquery}");
		}

		if (offset > 0)
		{
			queryParams.Add($"offset={offset}");
		}

		if (limit > 0)
		{
			queryParams.Add($"rowCount={limit}");
		}

		if (queryParams.Count > 0)
		{
			requestUri += $"?{string.Join("&", queryParams)}";
		}

		_logger.LogDebug("Querying serilog: {requestUri}", requestUri);
		var response = await httpClient.GetAsync(requestUri);
		response.EnsureSuccessStatusCode();

		var jsonContent = await response.Content.ReadAsStringAsync();
		var entries = JsonSerializer.Deserialize<SerilogEntry[]>(jsonContent, JsonSerializerOptions.Web);

		return entries ?? [];
	}

	public async Task<SourceContextMetricsResult[]> GetMetricsAsync()
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(_endpoint);
		ArgumentException.ThrowIfNullOrWhiteSpace(_headerSecret);

		using var httpClient = InitClient();

		var requestUri = "metrics";

		_logger.LogDebug("Querying serilog metrics: {requestUri}", requestUri);
		var response = await httpClient.GetAsync(requestUri);
		response.EnsureSuccessStatusCode();

		var jsonContent = await response.Content.ReadAsStringAsync();
		var metrics = JsonSerializer.Deserialize<SourceContextMetricsResult[]>(jsonContent, JsonSerializerOptions.Web);
		
		return metrics ?? [];
	}

	// todo: log levels
}
