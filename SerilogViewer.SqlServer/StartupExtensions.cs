using Coravel;
using Coravel.Scheduling.Schedule.Event;
using Coravel.Scheduling.Schedule.Interfaces;
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

	public static void RunSerilogCleanup(this IServiceProvider serviceProvider, Action<IScheduleInterval> config)
	{
		serviceProvider.UseScheduler(scheduler =>
		{
			var schedule = scheduler.Schedule<SerilogCleanup>();
			config(schedule); // let caller choose EveryMinute(), Daily(), etc.
		});
	}
}
