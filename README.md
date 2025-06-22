# Problem Statement

When self-hosting Serilog, there's no built-in log view experience. You have to query the serilog table manually and/or build your own UI or query feature. There are products like Seq and Graylog that provide a very polished log search and view exeperience, but have costs of their own. The reason for self-hosting to begin with is to avoid those costs. Furthermore, traditional homegrown log viewers have not provided the kind of insights I'm looking for when examining logs. I'd like a log view that helps focus my attention on distinct issues. There's still a place for the traditional scrolling table view, but one thing I want to do here is to improve upon this classic view with a card view of distinct recurring issues.

A secondary limitation of Serilog IMO is that the `SourceContext` property is not treated as a built-in column but rather as a custom property. As a custom property, it's not easy enough to search by or present in a UI. Although you can customize the configuration to capture this as a column, this has to be done for each application. I'd like to provide a standard way to capture this with minimal customization.

So this project does these things:
- indexes exception info in order to help you focus on recurring issues
- provides some Razor components for viewing Serilog detail in your applications
- provides a way to capture custom properties as columns with minimal configuration

## Exception Indexing
Exception "indexing" means extracting key info from stack traces and storing it in custom EF Core tables in order to provide visibility on recurring exceptions, and more specifically to help you drill down to root causes of exceptions. Raw stack traces have a lot of information and are hard to read at a glance.
- [StackTraceInfo](https://github.com/adamfoneil/SerilogViewer/blob/master/Parsing/StackTraceInfo.cs) is the info model
- [ExceptionIndexer](https://github.com/adamfoneil/SerilogViewer/blob/master/Parsing/ExceptionIndexer.cs) is the background process that periodically scans your Serilog table for exception info, using [Coravel](https://docs.coravel.net/)
- custom tables are defined in [IndexedLogContent](https://github.com/adamfoneil/SerilogViewer/tree/master/Parsing/IndexedLogContext)
