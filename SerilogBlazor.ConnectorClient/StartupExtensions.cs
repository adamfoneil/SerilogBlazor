using Microsoft.Extensions.DependencyInjection;

namespace SerilogBlazor.ConnectorClient;

public static class StartupExtensions
{
	public static void AddSerilogConnectorClientFactory(this IServiceCollection services)
	{
		services.AddHttpClient();
		services.AddSingleton<ConnectorClientFactory>();
	}
}
