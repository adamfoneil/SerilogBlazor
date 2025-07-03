This is a collection of Blazor components to improve your Serilog experience, currently targeting SQL Server and Blazor Server.

**Log level changer:**

Use this to change log levels at runtime in your app for whatever namespaces/source contexts you define. This is handy when you need to temporarily increase log detail in a specific area to troubleshoot a production issue.

![image](https://github.com/user-attachments/assets/aa45b46f-0fe3-4814-ab36-f097ca1f9c5a)


**Grid view:**

Scrolling grid of log entries.

![image](https://github.com/user-attachments/assets/24655719-9ff2-473f-9745-87e4b1ebd8e5)

Expands to show exception detail and properties.

![image](https://github.com/user-attachments/assets/ece06237-93bd-4d41-b6e2-6ee503e217af)

**Source Context Filter**

Zoom out to see total log entries by level and source context.

![image](https://github.com/user-attachments/assets/3163a2d9-77e3-4f1d-855a-36ffbb1a1427)


**Search Bar**

Search your logs with a variety of shortcuts. Save searches for easy reuse.

![image](https://github.com/user-attachments/assets/d11a83e8-4e30-4dde-bf74-f468aff20528)


# Problem Statement

When self-hosting Serilog, there's no built-in log view experience. You have to query the serilog table manually and/or build your own UI or query feature. There are products like Seq and Graylog that provide a very polished log search and view exeperience, but have costs of their own. The reason for self-hosting to begin with is to avoid those costs. Furthermore, traditional homegrown log viewers have not provided the kind of insights and capabilities I'm looking for when examining logs. The goal for this project is to build the most capable log view experience I can, addressing longstanding pain points I've come across -- then offer it as a Razor Class Library NuGet package. 

With .NET Aspire coming online recently, it has potential to disrupt and improve logging in ASP.NET Core due to its integrated open telemetry support and viewer dashboards. So, I'm very late to this party, and this project may be irrelevant in the short term. But I'm not convinced yet that Aspire's logging/otel does everything I want it to. Furthermore, many apps implement Serilog already, and I think there's a case for meeting apps where they are rather than pushing them to implement Aspire. (I want Aspire to succeed, and am happy to keep tabs on it as it evolves.)

Following is a list of logging pain points and how this project addresses them.

<details>
  <summary>Unexpected log levels</summary>
  
  Over the years, I've had a hard time getting log levels and namespaces right -- that is, getting the desired level of logging at the right places in my code. Also, I didn't know until recently know there was a way to change levels at runtime without restarting my apps.

  This project does these things:
  - Offers the [LogLevels](https://github.com/adamfoneil/SerilogViewer/blob/master/SerilogViewer.Abstractions/LogLevels.cs) abstract class.
  - Implement this in your project to define your default logging levels by namespace prefix. Sample implementation is [ApplicationLogLevels](https://github.com/adamfoneil/SerilogViewer/blob/master/SampleApp/ApplicationLogLevels.cs).
  - Configure levels at runtime via the [LevelToggle](https://github.com/adamfoneil/SerilogViewer/blob/master/SerilogViewer.RCL/LevelToggle.razor) component
  
  

  You can see which levels are in effect on which namespaces via the [SourceContextFilter](https://github.com/adamfoneil/SerilogViewer/blob/e83c1c5927c03bc47f8a0eecc70d097eaf513f23/SerilogViewer.RCL/SourceContextFilter.razor#L26).
  
  ![image](https://github.com/user-attachments/assets/953c275c-a31f-440a-9e34-0597fad0c79d)

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
  <summary>Stack traces are too hard to read</summary>
</details>
