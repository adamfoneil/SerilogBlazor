using Coravel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SandboxCmd;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using Service;

var host = Host.CreateDefaultBuilder(args)
	.UseSerilog((context, services, configuration) =>
	{
		configuration
			.WriteTo.Console()
			.WriteTo.MSSqlServer(AppDbContextFactory.ConnectionString, new MSSqlServerSinkOptions()
			{
				AutoCreateSqlTable = true,
				TableName = "Serilog",
				SchemaName = "log"
			});

		services.UseScheduler(schedule =>
		{
			schedule.Schedule<SerilogIndexer<ApplicationDbContext>>().EveryThirtyMinutes();
		});
	})
	.ConfigureServices((context, services) =>
	{		
		services.AddDbContextFactory<ApplicationDbContext>(options => options.UseSqlServer(AppDbContextFactory.ConnectionString));
	})
	.Build();

using var services = host.Services.CreateScope();


await host.RunAsync();