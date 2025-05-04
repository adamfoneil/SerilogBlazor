using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SandboxCmd;
using Serilog;
using Serilog.Sinks.MSSqlServer;

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
	})
	.ConfigureServices((context, services) =>
	{
		services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(AppDbContextFactory.ConnectionString));
	})
	.Build();

await host.RunAsync();