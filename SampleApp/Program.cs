using Microsoft.EntityFrameworkCore;
using SampleApp;
using SampleApp.Components;
using SampleApp.Data;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using SerilogBlazor.Abstractions;
using SerilogBlazor.SqlServer;

var logLevels = new ApplicationLogLevels();

Log.Logger = logLevels
	.GetConfiguration()
	.MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Information)
	.WriteTo.Console()
	.WriteTo.MSSqlServer(AppDbContextFactory.ConnectionString, new MSSqlServerSinkOptions()
	{
		AutoCreateSqlTable = true,
		TableName = "Serilog",
		SchemaName = "log",
	}, columnOptions: SqlServerColumnOptions.Default)
	.Enrich.FromLogContext()
	.CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<ApplicationDbContext>(config => config.UseSqlServer(AppDbContextFactory.ConnectionString), ServiceLifetime.Singleton);

builder.Services.AddSerilogUtilities(AppDbContextFactory.ConnectionString, logLevels, "log", "Serilog", TimestampType.Local);

builder.Services.AddSerilogCleanup(new() 
{ 
	ConnectionString = AppDbContextFactory.ConnectionString, 
	TableName = "log.Serilog",
	Debug = 5,
	Information = 20,
	Warning = 20,
	Error = 20
});

builder.Services.AddScoped<SampleService>();

var app = builder.Build();

app.Services.RunSerilogCleanup(interval => interval.DailyAt(0, 0)); 

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
