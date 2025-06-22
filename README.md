# Problem Statement

When self-hosting Serilog, there's no built-in log view experience. You have to query the serilog table manually and/or build your own UI or query feature. There are products like Seq and Graylog that provide a very polished log search and view exeperience, but have costs of their own. The reason for self-hosting to begin with is to avoid those costs. Furthermore, traditional homegrown log viewers have not provided the kind of insights I'm looking for when examining logs. I'd like a log view that helps focus my attention on distinct issues. There's still a place for the traditional scrolling table view, but one thing I want to do here is to improve upon this classic view with a card view of distinct recurring issues.

A secondary limitation of Serilog IMO is that the `SourceContext` property is not treated as a built-in column but rather as a custom property. Although you can customize the configuration to capture this as a column, this has to be done for each sink you configure. I'd like to provide an easier way to capture `SourceContext` without having to customize the Serilog configuration in an application.

So this project does two things:
- provides some Blazor components
