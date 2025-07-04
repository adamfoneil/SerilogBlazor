using Coravel;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SerilogBlazor.Abstractions;

namespace SerilogBlazor.Postgres;

public static class StartupExtensions
{
	public static void AddSerilogUtilities(this IServiceCollection services, 
		string connectionString, LogLevels logLevels, 
		string schemaName = "public", string tableName = "Logs", string timezone)
	{
		services.AddSingleton(logLevels);
		services.AddSingleton<LoggingRequestIdProvider>();

		services.AddSingleton<SerilogSourceContextMetricsQuery>(sp =>
			new SerilogPostgresSourceContextMetricsQuery(
				timezone,
				sp.GetRequiredService<ILogger<SerilogPostgresSourceContextMetricsQuery>>(),
				sp.GetRequiredService<LoggingRequestIdProvider>(),
				connectionString, schemaName, tableName
		));

		services.AddSingleton<SerilogQuery>(sp =>
			new SerilogPostgresQuery(				
				timezone,
				sp.GetRequiredService<ILogger<SerilogPostgresQuery>>(),
				sp.GetRequiredService<LoggingRequestIdProvider>(),
				connectionString, schemaName, tableName
		));
	}

	public static void AddSerilogCleanup(this IServiceCollection services, SerilogCleanupOptions options)
	{
		services.AddSingleton<LoggingRequestIdProvider>();

		services.AddScheduler();

		services.AddSingleton(Options.Create(options));

		services.AddSingleton<SerilogCleanup>(sp =>
			new SerilogPostgresCleanup(
				sp.GetRequiredService<LoggingRequestIdProvider>(),				
				sp.GetRequiredService<ILogger<SerilogPostgresCleanup>>(),
				sp.GetRequiredService<IOptions<SerilogCleanupOptions>>()
		));
	}

	public static void RunSerilogCleanup(this IServiceProvider serviceProvider, Action<IScheduleInterval> config)
	{
		serviceProvider.UseScheduler(scheduler =>
		{
			var schedule = scheduler.Schedule<SerilogCleanup>();
			config(schedule); // let caller choose EveryMinute(), Daily(), etc.
		});
	}
}