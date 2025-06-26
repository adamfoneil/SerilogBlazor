using Coravel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SerilogViewer.Abstractions;

namespace SerilogViewer.SqlServer;

public static class StartupExtensions
{
	public static void AddSerilogCleanup(this IServiceCollection services, SerilogCleanupOptions options)
	{
		services.AddScheduler();

		services.AddSingleton(Options.Create(options));

		services.AddSingleton<SerilogCleanup>(sp =>
			new SerilogSqlServerCleanup(
				sp.GetRequiredService<LoggingRequestIdProvider>(),				
				sp.GetRequiredService<ILogger<SerilogSqlServerCleanup>>(),
				sp.GetRequiredService<IOptions<SerilogCleanupOptions>>()
			));
	}

	
}
