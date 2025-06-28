using Coravel;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SerilogViewer.Abstractions;

namespace SerilogViewer.SqlServer;

public static class StartupExtensions
{
	public static void AddSerilogQuery(this IServiceCollection services, string connectionString, string schemaName = "dbo", string tableName = "Serilog", TimestampType timestampType = TimestampType.Utc)
	{
		services.AddSingleton<LoggingRequestIdProvider>();

		services.AddSingleton<SerilogQuery>(sp =>
			new SerilogSqlServerQuery(				
				timestampType,
				sp.GetRequiredService<ILogger<SerilogSqlServerQuery>>(),
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
			new SerilogSqlServerCleanup(
				sp.GetRequiredService<LoggingRequestIdProvider>(),				
				sp.GetRequiredService<ILogger<SerilogSqlServerCleanup>>(),
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
