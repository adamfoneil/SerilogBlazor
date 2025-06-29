# Problem Statement

When self-hosting Serilog, there's no built-in log view experience. You have to query the serilog table manually and/or build your own UI or query feature. There are products like Seq and Graylog that provide a very polished log search and view exeperience, but have costs of their own. The reason for self-hosting to begin with is to avoid those costs. Furthermore, traditional homegrown log viewers have not provided the kind of insights and capabilities I'm looking for when examining logs. The goal for this project is to build the most capable log view experience I can, addressing longstanding pain points I've come across -- then offer it as a Razor Class Library NuGet package. 

With .NET Aspire coming online recently, it has potential to disrupt and improve logging in ASP.NET Core due to its integrated open telemetry support and viewer dashboards. So, I'm very late to this party, and this project may be irrelevant in the short term. But I'm not convinced yet that Aspire's logging/otel does everything I want it to. Furthermore, many apps implement Serilog already, and I think there's a case for meeting apps where they are rather than pushing them to implement Aspire. (I want Aspire to succeed, and am happy to keep tabs on it as it evolves.)

Following is a list of logging pain points and how this project addresses them.

<details>
  <summary>Unexpected log levels</summary>
</details>

<details>
  <summary>Difficulties with correlation/deep tracing</summary>
</details>

<details>
  <summary>Query limitations due to XML dependency in SQL Server sink</summary>
</details>

<details>
  <summary>Logs getting too big</summary>
</details>

<details>
  <summary>No built-in alerting</summary>
</details>

<details>
  <summary>Stack traces have too much info, are hard to make sense of</summary>
</details>

I'd like a log view that helps focus my attention on distinct issues. There's still a place for the traditional scrolling table view, but one thing I want to do here is to improve upon this classic view with a card view of distinct recurring issues.

The other difficulty I run into with Serilog is configuring the logging levels by namespace. These are string arguments with no feedback or preview capability, and changes require app restarts. So I'm looking for an improved configuration experience of some kind.

So this project does these things:
- indexes exception info in order to help you focus on recurring issues
- provides some Razor components for viewing Serilog detail in your applications

## Exception Indexing
Exception "indexing" means extracting key info from stack traces and storing it in custom EF Core tables in order to provide visibility on recurring exceptions, and more specifically to help you drill down to root causes of exceptions. Raw stack traces have a lot of information and are hard to read at a glance.
- [StackTraceInfo](https://github.com/adamfoneil/SerilogViewer/blob/master/Parsing/StackTraceInfo.cs) is the info model
- [ExceptionIndexer](https://github.com/adamfoneil/SerilogViewer/blob/master/Parsing/ExceptionIndexer.cs) is the background process that periodically scans your Serilog table for exception info, using [Coravel](https://docs.coravel.net/)
- custom tables are defined in [IndexedLogContent](https://github.com/adamfoneil/SerilogViewer/tree/master/Parsing/IndexedLogContext)

## Tracing
Todo...

## Dynamic Log Levels
Todo...

## Log Viewer Components
Todo...
