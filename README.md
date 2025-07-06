This is a collection of Blazor components to improve your Serilog experience, currently targeting SQL Server and Blazor Server. NuGet packages:

- [Serilog.Blazor.SqlServer](https://www.nuget.org/packages/Serilog.Blazor.SqlServer)
- [Serilog.Blazor.Postgres](https://www.nuget.org/packages/Serilog.Blazor.Postgres)
- [Serilog.Blazor.RCL](https://www.nuget.org/packages/Serilog.Blazor.RCL)

# Components

<details>
  <summary>LevelToggle</summary>
  
  Use this to change log levels at runtime in your app for whatever namespaces/source contexts you define. This is handy when you need to temporarily increase log detail in a specific area to troubleshoot a production issue.

  ![image](https://github.com/user-attachments/assets/aa45b46f-0fe3-4814-ab36-f097ca1f9c5a)

  You define your own levels of course. Screenshot above is just a sample.

  Source: [LevelToggle](https://github.com/adamfoneil/SerilogBlazor/blob/master/SerilogBlazor.RCL/LevelToggle.razor)
  
  Example: [SampleApp](https://github.com/adamfoneil/SerilogBlazor/blob/f7d98814e280582c8d1ffbe32e5e4b5a1b0ab7b3/SampleApp/Components/Pages/Home.razor#L14)
  
</details>

<details>
  <summary>SerilogGrid</summary>

  Scrolling grid of log entries.

![image](https://github.com/user-attachments/assets/24655719-9ff2-473f-9745-87e4b1ebd8e5)

Expands to show exception detail and properties.

![image](https://github.com/user-attachments/assets/ece06237-93bd-4d41-b6e2-6ee503e217af)

Source: [SerilogGrid](https://github.com/adamfoneil/SerilogBlazor/blob/master/SerilogBlazor.RCL/SerilogGrid.razor)

Example: [SampleApp](https://github.com/adamfoneil/SerilogBlazor/blob/f7d98814e280582c8d1ffbe32e5e4b5a1b0ab7b3/SampleApp/Components/Pages/Home.razor#L19)

</details>

<details>
  <summary>SourceContextFilter</summary>

Zoom out to see total log entries by level and source context.

![image](https://github.com/user-attachments/assets/3163a2d9-77e3-4f1d-855a-36ffbb1a1427)

Source: [SourceContextFilter](https://github.com/adamfoneil/SerilogBlazor/blob/master/SerilogBlazor.RCL/SourceContextFilter.razor)

Example: [SampleApp](https://github.com/adamfoneil/SerilogBlazor/blob/f7d98814e280582c8d1ffbe32e5e4b5a1b0ab7b3/SampleApp/Components/Pages/Home.razor#L17)

</details>

<details>
  <summary>SearchBar</summary>

Search your logs with a variety of shortcuts. Save searches for easy reuse. Supported syntax:
- enclose text in square brackets to search the **source context** field.
- use a pound sign prefix to search the **request Id** property.
- use the @ sign prefix to search the log level, e.g. `@warn` or `@err` or `@info`
- use a minus sign prefix followed by number and duration unit, e.g. `-15m` for within 15 minutes or `-1d` for one day ago

![image](https://github.com/user-attachments/assets/d11a83e8-4e30-4dde-bf74-f468aff20528)

Source: [SearchBar](https://github.com/adamfoneil/SerilogBlazor/blob/master/SerilogBlazor.RCL/SearchBar.razor)

Example: [SampleApp](https://github.com/adamfoneil/SerilogBlazor/blob/f7d98814e280582c8d1ffbe32e5e4b5a1b0ab7b3/SampleApp/Components/Pages/Home.razor#L18)

This has a saved search feature that requires an EF Core DbContext implementing this interface [ISerilogSavedSearches](https://github.com/adamfoneil/SerilogBlazor/blob/master/SerilogBlazor.Abstractions/SavedSearches/ISerilogSavedSearches.cs).

After implementing this interface on your DbContext, add a migration to add the underlying table to your database.

</details>

# Getting Started (SQL Server)

1. Install the SQL Server and RCL packages listed above. (Also the [Serilog.Sinks.MSSqlServer](https://www.nuget.org/packages/Serilog.Sinks.MSSqlServer) package if you don't have it yet.)
2. Implement abstract class [LogLevels](https://github.com/adamfoneil/SerilogBlazor/blob/master/SerilogBlazor.Abstractions/LogLevels.cs) in your app. Example: [ApplicationLogLevels](https://github.com/adamfoneil/SerilogBlazor/blob/master/SampleApp/ApplicationLogLevels.cs)
3. In your app startup, create your `ApplicationLogLevels` instance (or whatever you decide to call it), and use it as the basis of your Serilog configuration. Also be sure to include the [SqlServerColumnOptions.Default](https://github.com/adamfoneil/SerilogBlazor/blob/master/SerilogBlazor.SqlServer/ColumnOptions.cs) `columnOptions` argument. This ensures the `SourceContext` is captured as a dedicated column in your logs. Example:

```csharp
var logLevels = new ApplicationLogLevels();

Log.Logger = logLevels
  .GetConfiguration()  
  .WriteTo.MSSqlServer({your connection string}, new MSSqlServerSinkOptions()
  {
    AutoCreateSqlTable = true,
    TableName = "Serilog", // whatever table name you like
    SchemaName = "log", // whatever schema you like
  }, columnOptions: SqlServerColumnOptions.Default) // this is important
  .Enrich.FromLogContext()
  .CreateLogger();
```
4. If using the SearchBar, add an EF Core `IDbContextFactory<T>` to your startup. [Example](https://github.com/adamfoneil/SerilogBlazor/blob/f7d98814e280582c8d1ffbe32e5e4b5a1b0ab7b3/SampleApp/Program.cs#L32).

4. Call extension method [AddSerilogUtilities](https://github.com/adamfoneil/SerilogBlazor/blob/f7d98814e280582c8d1ffbe32e5e4b5a1b0ab7b3/SerilogBlazor.SqlServer/StartupExtensions.cs#L12) in your app startup. Example:

```csharp
// to enable search bar saved searches. You might already have a db context being added somewhere, but it needs to be a factory specifically for this library. Lifetime doesn't matter. I use singleton here, but it can be scoped
builder.Services.AddDbContextFactory<ApplicationDbContext>(config => config.UseSqlServer({your connection string}), ServiceLifetime.Singleton);

// adds log level toggle and infrastructure for querying Serilog table. Use your serilog table name and schema. Also, chang ethe TimestampType according to how log entries are timestamped. I'm using Local in the example here
builder.Services.AddSerilogUtilities({your connection string}, logLevels, "log", "Serilog", TimestampType.Local);
``` 

# Getting Started (Postgres)
1. Install the Postgres and RCL packages listed above.
2. Implement abstract class [LogLevels](https://github.com/adamfoneil/SerilogBlazor/blob/master/SerilogBlazor.Abstractions/LogLevels.cs) in your app.
3. In your app startup, initialize Serilog with user your `LogLevels` class (whatever you call it) and call a couple of the provided extension methods. Note the use of custom column options [PostgresColumnOptions](https://github.com/adamfoneil/SerilogBlazor/blob/master/SerilogBlazor.Postgres/ColumnOptions.cs).

```csharp
var logLevels = new ApplicationLogLevels();

Log.Logger = logLevels
	.GetConfiguration()
	.WriteTo.Console() // optional, but I use this
	.WriteTo.PostgreSQL({your connection string}, "serilog", columnOptions: PostgresColumnOptions.Default, needAutoCreateTable: true)	
	.Enrich.FromLogContext()	
	.CreateLogger();

builder.Services.AddSerilog();
builder.Services.AddSerilogUtilities({your connection string}, logLevels, "public", "serilog", TimestampType.Utc);
```
4. After initial startup for the first time, run this SQL script on your Postgres database to add the `id` column. It's not created by default when you use custom column options.

```sql
ALTER TABLE serilog ADD id serial PRIMARY KEY
```

Now your ready to build your own Serilog viewer page. Refer to [sample](https://github.com/adamfoneil/SerilogBlazor/blob/master/SampleApp/Components/Pages/Home.razor) above for ideas.

# Goodies and Extensions

## BeginRequestId
Correlating log entries in API or non-SPA web apps can be done with ASP.NET Core's `HttpContext.TraceIdentifier` property. This is not helpful in Blazor due to how it works with `HttpContext`. As an alternative, this library provides a generic log correlation extension method [BeginRequestId](https://github.com/adamfoneil/SerilogBlazor/blob/728e242bc2d91bf10779831ba843587c0c2e4631/SerilogBlazor.Abstractions/LoggerExtensions.cs#L10). Use this at the beginning of a method where you want to ensure that all logs written in that scope have a common identifier. [Example](https://github.com/adamfoneil/SerilogBlazor/blob/728e242bc2d91bf10779831ba843587c0c2e4631/SampleApp/Components/Pages/Home.razor#L30). This works with [LoggingRequestIdProvider](https://github.com/adamfoneil/SerilogBlazor/blob/master/SerilogBlazor.Abstractions/LoggingRequestIdProvider.cs) to provide an auto-incrementing value when it's called.

```csharp
@inject LoggingRequestIdProvider RequestId

private void LogThis()
{
  // attach an id to all logging in this method
  Logger.BeginRequestId(RequestId);

  Logger.LogInformation("This is an info log message");

  Logger.LogDebug("This is a debug message");

  // logs from this method call will be correlated with requestId in scope here
  SampleService.DoWork();
}
```

## SerilogCleanup
Implement retention periods for various log levels with the `AddSerilogCleanup` method, used at startup:

```csharp
builder.Services.AddSerilogCleanup(new() 
{ 
  ConnectionString = {your connection string}, 
  TableName = "log.Serilog", // your serilog table name
  Debug = 5, // retention period in days
  Information = 20,
  Warning = 20,
  Error = 20
});
```

Then in your pipeline, call `RunSerilogCleanup` with a desired interval. This uses [Coravel Scheduling](https://docs.coravel.net/Scheduler/), so use its syntax for setting the interval.

```csharp
var app = builder.Build();

app.Services.RunSerilogCleanup(interval => interval.DailyAt(0, 0)); 
```

# Motivation

When self-hosting Serilog, there's no built-in log view experience. You have to query the serilog table manually and/or build your own UI or query feature. There are products like Seq and Graylog that provide a very polished log search and view exeperience, but have costs of their own. The reason for self-hosting to begin with is to avoid those costs. Furthermore, traditional homegrown log viewers have not provided the kind of insights and capabilities I'm looking for when examining logs. The goal for this project is to build the most capable log view experience I can, addressing longstanding pain points I've come across -- then offer it as a Razor Class Library NuGet package. 

With .NET Aspire coming online recently, it has potential to disrupt and improve logging in ASP.NET Core due to its integrated open telemetry support and viewer dashboards. So, I'm very late to this party, and this project may be irrelevant in the short term. But I'm not convinced yet that Aspire's logging/otel does everything I want it to. Furthermore, many apps implement Serilog already, and I think there's a case for meeting apps where they are rather than pushing them to implement Aspire. (I want Aspire to succeed, and am happy to keep tabs on it as it evolves.)
