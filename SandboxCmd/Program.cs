using Coravel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SandboxCmd;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using Service;
using System.Data;

var host = Host.CreateDefaultBuilder(args)
	.UseSerilog((context, services, configuration) =>
	{
		configuration
			.WriteTo.Console()
			.WriteTo.MSSqlServer(AppDbContextFactory.ConnectionString, new MSSqlServerSinkOptions()
			{
				AutoCreateSqlTable = true,
				TableName = "Serilog",
				SchemaName = "log",				
			}, columnOptions: GetColumnOptions());

		services.UseScheduler(schedule =>
		{
			schedule.Schedule<ExceptionIndexer<ApplicationDbContext>>().EveryThirtyMinutes();
		});
	})
	.ConfigureServices((context, services) =>
	{
		services.AddScoped<DemoService>();
		services.AddScheduler();
		services.AddDbContextFactory<ApplicationDbContext>(options => options.UseSqlServer(AppDbContextFactory.ConnectionString));
	})
	.Build();

static ColumnOptions GetColumnOptions() => new()
{
	AdditionalColumns =
	[
		new SqlColumn("SourceContext", SqlDbType.NVarChar, allowNull: true, dataLength: 256),
	]
};

await host.RunAsync();