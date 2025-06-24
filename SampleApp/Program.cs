using SampleApp.Components;
using SampleApp.Data;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Data;

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.WriteTo.MSSqlServer(AppDbContextFactory.ConnectionString, new MSSqlServerSinkOptions()
	{
		AutoCreateSqlTable = true,
		TableName = "Serilog",
		SchemaName = "log",
	}, columnOptions: GetColumnOptions())
	.Enrich.FromLogContext()
	.CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();


// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

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

static ColumnOptions GetColumnOptions() => new()
{
	AdditionalColumns =
	[
		new SqlColumn("SourceContext", SqlDbType.NVarChar, allowNull: true, dataLength: 256),
	]
};
