using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SerilogBlazor.ConnectorClient;

public class ConnectorClientFactory(IServiceProvider serviceProvider)
{
	private readonly IServiceProvider _serviceProvider = serviceProvider;

	public SerilogApiConnectorClient CreateClient(string endpoint, string headerSecret)
	{
		using var scope = _serviceProvider.CreateScope();
		var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
		var logger = scope.ServiceProvider.GetRequiredService<ILogger<SerilogApiConnectorClient>>();
		return new SerilogApiConnectorClient(logger, httpClientFactory, endpoint, headerSecret);
	}
}
