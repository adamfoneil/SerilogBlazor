using SerilogBlazor.Abstractions;
using System.Text.Json;

namespace SerilogBlazor.ConnectorClient;

public class SerilogApiConnectorClient(IHttpClientFactory httpClientFactory)
{
	private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

	public async Task<SerilogEntry[]> GetEntriesAsync(string endpoint, string headerSecret, string? criteria = null)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(endpoint);
		ArgumentException.ThrowIfNullOrWhiteSpace(headerSecret);

		using var httpClient = _httpClientFactory.CreateClient();
		httpClient.DefaultRequestHeaders.Add("serilog-api-secret", headerSecret);

		var requestUri = $"{endpoint.TrimEnd('/')}/detail";
		
		if (!string.IsNullOrWhiteSpace(criteria))
		{
			var encodedCriteria = Uri.EscapeDataString(criteria);
			requestUri += $"?search={encodedCriteria}";
		}

		var response = await httpClient.GetAsync(requestUri);
		response.EnsureSuccessStatusCode();

		var jsonContent = await response.Content.ReadAsStringAsync();
		var entries = JsonSerializer.Deserialize<SerilogEntry[]>(jsonContent, JsonSerializerOptions.Web);
		
		return entries ?? [];
	}

	public async Task<SourceContextMetricsResult[]> GetMetricsAsync(string endpoint, string headerSecret)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(endpoint);
		ArgumentException.ThrowIfNullOrWhiteSpace(headerSecret);

		using var httpClient = _httpClientFactory.CreateClient();
		httpClient.DefaultRequestHeaders.Add("serilog-api-secret", headerSecret);

		var requestUri = $"{endpoint.TrimEnd('/')}/metrics";

		var response = await httpClient.GetAsync(requestUri);
		response.EnsureSuccessStatusCode();

		var jsonContent = await response.Content.ReadAsStringAsync();
		var metrics = JsonSerializer.Deserialize<SourceContextMetricsResult[]>(jsonContent, JsonSerializerOptions.Web);
		
		return metrics ?? [];
	}

	// todo: log levels
}
