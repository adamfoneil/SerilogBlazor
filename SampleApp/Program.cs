using SampleApp;
using SampleApp.Components;
using SampleApp.Data;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using SerilogViewer.Abstractions;
using SerilogViewer.SqlServer;

Log.Logger = new LoggerConfiguration()
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

builder.Services.AddSingleton<LoggingRequestIdProvider>();
builder.Services.AddScoped<SampleService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
