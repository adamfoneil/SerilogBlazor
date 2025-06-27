using SampleApp;
using SampleApp.Components;
using Serilog;
using SerilogViewer.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Simple console logging only
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Services.AddSingleton(new ApplicationLogLevels());
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<LoggingRequestIdProvider>();
builder.Services.AddScoped<SampleService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();