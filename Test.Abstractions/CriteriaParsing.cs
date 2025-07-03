using SerilogBlazor.Abstractions;

namespace Testing;

[TestClass]
public class CriteriaParsing
{
	[TestMethod]
	public void MessageOnly()
	{
		var input = "unhandled exception";
		var output = SerilogQuery.Criteria.ParseExpression(input);
		Assert.IsTrue(output is { Message: "unhandled exception" });
	}

	[TestMethod]
	public void MessageWithAge()
	{				
		var input = "unhandled exception -3d";
		var output = SerilogQuery.Criteria.ParseExpression(input);
		Assert.AreEqual("unhandled exception", output.Message);
		Assert.AreEqual(TimeSpan.FromDays(3), output.Age);
	}

	[TestMethod]
	public void ErrorsWithAgeAndSourceContext()
	{
		var input = "@err -3hr [MyApp]";
		var output = SerilogQuery.Criteria.ParseExpression(input);
		Assert.AreEqual("Error", output.Level);
		Assert.AreEqual(TimeSpan.FromHours(3), output.Age);
		Assert.AreEqual("MyApp", output.SourceContext);
	}

	[TestMethod]
	public void InfoWithAgeAndRequestId()
	{
		var input = "@info -1d #12345";
		var output = SerilogQuery.Criteria.ParseExpression(input);
		Assert.AreEqual("Info", output.Level);
		Assert.AreEqual(TimeSpan.FromDays(1), output.Age);
		Assert.AreEqual("12345", output.RequestId);
	}

	[TestMethod]
	public void RequestIdWithMessage()
	{
		var input = "#abc-123 with message";
		var output = SerilogQuery.Criteria.ParseExpression(input);
		Assert.AreEqual("abc-123", output.RequestId);
		Assert.AreEqual("with message", output.Message);
	}

	[TestMethod]
	public void ExceptionText()
	{
		var input = "!NullReferenceException \"dandy warhols\"";
		var output = SerilogQuery.Criteria.ParseExpression(input);
		Assert.AreEqual("NullReferenceException", output.Exception);
		Assert.AreEqual("dandy warhols", output.Message);
	}

	[TestMethod]
	public void WeeklyAge()
	{
		var input = "error logs -2wk";
		var output = SerilogQuery.Criteria.ParseExpression(input);
		Assert.AreEqual("error logs", output.Message);
		Assert.AreEqual(TimeSpan.FromDays(14), output.Age); // 2 weeks = 14 days
	}

	[TestMethod]
	public void MonthlyAge()
	{
		var input = "error logs -3mon";
		var output = SerilogQuery.Criteria.ParseExpression(input);
		Assert.AreEqual("error logs", output.Message);
		Assert.AreEqual(TimeSpan.FromDays(90), output.Age); // 3 months = 90 days
	}

	[TestMethod]
	public void SingleWeekAge()
	{
		var input = "-1wk";
		var output = SerilogQuery.Criteria.ParseExpression(input);
		Assert.IsNull(output.Message);
		Assert.AreEqual(TimeSpan.FromDays(7), output.Age); // 1 week = 7 days
	}

	[TestMethod]
	public void SingleMonthAge()
	{
		var input = "-1mon";
		var output = SerilogQuery.Criteria.ParseExpression(input);
		Assert.IsNull(output.Message);
		Assert.AreEqual(TimeSpan.FromDays(30), output.Age); // 1 month = 30 days
	}

	[TestMethod]
	public void MixedCriteriaWithWeeks()
	{
		var input = "@warn -2wk [MyService]";
		var output = SerilogQuery.Criteria.ParseExpression(input);
		Assert.AreEqual("Warning", output.Level);
		Assert.AreEqual(TimeSpan.FromDays(14), output.Age);
		Assert.AreEqual("MyService", output.SourceContext);
	}

	[TestMethod]
	public void RequestIdReplacement()
	{
		// Test that the regex pattern used in SearchBar.AddRequestId correctly removes existing RequestIds
		var regex = new System.Text.RegularExpressions.Regex(@"#\w+");
		
		// Test with existing RequestId in middle
		var input1 = "error message #oldId more text";
		var cleaned1 = regex.Replace(input1, "").Trim();
		Assert.AreEqual("error message  more text", cleaned1);
		
		// Test with RequestId at the beginning
		var input2 = "#oldId error message";
		var cleaned2 = regex.Replace(input2, "").Trim();
		Assert.AreEqual("error message", cleaned2);
		
		// Test with RequestId at the end
		var input3 = "error message #oldId";
		var cleaned3 = regex.Replace(input3, "").Trim();
		Assert.AreEqual("error message", cleaned3);
		
		// Test with no RequestId
		var input4 = "error message";
		var cleaned4 = regex.Replace(input4, "").Trim();
		Assert.AreEqual("error message", cleaned4);
		
		// Test with multiple RequestIds (edge case)
		var input5 = "#first error #second message";
		var cleaned5 = regex.Replace(input5, "").Trim();
		Assert.AreEqual("error  message", cleaned5);
	}

	[TestMethod]
	public void SourceContextReplacement()
	{
		// Test that the regex pattern used in SearchBar.AddSourceContext correctly removes existing SourceContexts
		var regex = new System.Text.RegularExpressions.Regex(@"\[[^\]]+\]");
		
		// Test with existing SourceContext in middle
		var input1 = "error message [OldApp] more text";
		var cleaned1 = regex.Replace(input1, "").Trim();
		Assert.AreEqual("error message  more text", cleaned1);
		
		// Test with SourceContext at the beginning
		var input2 = "[OldApp] error message";
		var cleaned2 = regex.Replace(input2, "").Trim();
		Assert.AreEqual("error message", cleaned2);
		
		// Test with SourceContext at the end
		var input3 = "error message [OldApp]";
		var cleaned3 = regex.Replace(input3, "").Trim();
		Assert.AreEqual("error message", cleaned3);
		
		// Test with no SourceContext
		var input4 = "error message";
		var cleaned4 = regex.Replace(input4, "").Trim();
		Assert.AreEqual("error message", cleaned4);
		
		// Test with multiple SourceContexts (edge case)
		var input5 = "[FirstApp] error [SecondApp] message";
		var cleaned5 = regex.Replace(input5, "").Trim();
		Assert.AreEqual("error  message", cleaned5);

		// Test with nested brackets (edge case)
		var input6 = "error [App.With.Dots] message";
		var cleaned6 = regex.Replace(input6, "").Trim();
		Assert.AreEqual("error  message", cleaned6);
	}

	[TestMethod]
	public void LogLevelReplacement()
	{
		// Test that the regex pattern used in SearchBar.AddSourceContext correctly removes existing LogLevels
		var regex = new System.Text.RegularExpressions.Regex(@"@@\w+");
		
		// Test with existing LogLevel in middle
		var input1 = "error message @@Error more text";
		var cleaned1 = regex.Replace(input1, "").Trim();
		Assert.AreEqual("error message  more text", cleaned1);
		
		// Test with LogLevel at the beginning
		var input2 = "@@Warning error message";
		var cleaned2 = regex.Replace(input2, "").Trim();
		Assert.AreEqual("error message", cleaned2);
		
		// Test with LogLevel at the end
		var input3 = "error message @@Information";
		var cleaned3 = regex.Replace(input3, "").Trim();
		Assert.AreEqual("error message", cleaned3);
		
		// Test with no LogLevel
		var input4 = "error message";
		var cleaned4 = regex.Replace(input4, "").Trim();
		Assert.AreEqual("error message", cleaned4);
		
		// Test with multiple LogLevels (edge case)
		var input5 = "@@Error message @@Warning text";
		var cleaned5 = regex.Replace(input5, "").Trim();
		Assert.AreEqual("message  text", cleaned5);
	}

	[TestMethod]
	public void SourceContextAndLogLevelReplacement()
	{
		// Test combined removal of both SourceContext and LogLevel
		var sourceContextRegex = new System.Text.RegularExpressions.Regex(@"\[[^\]]+\]");
		var logLevelRegex = new System.Text.RegularExpressions.Regex(@"@@\w+");
		
		// Test with both SourceContext and LogLevel
		var input1 = "error [OldApp] message @@Error more text";
		var cleaned1 = sourceContextRegex.Replace(input1, "");
		cleaned1 = logLevelRegex.Replace(cleaned1, "").Trim();
		Assert.AreEqual("error  message  more text", cleaned1);
		
		// Test with both at different positions
		var input2 = "@@Warning [MyApp] error message";
		var cleaned2 = sourceContextRegex.Replace(input2, "");
		cleaned2 = logLevelRegex.Replace(cleaned2, "").Trim();
		Assert.AreEqual("error message", cleaned2);
		
		// Test with only SourceContext
		var input3 = "error [App] message";
		var cleaned3 = sourceContextRegex.Replace(input3, "");
		cleaned3 = logLevelRegex.Replace(cleaned3, "").Trim();
		Assert.AreEqual("error  message", cleaned3);
		
		// Test with only LogLevel  
		var input4 = "error @@Debug message";
		var cleaned4 = sourceContextRegex.Replace(input4, "");
		cleaned4 = logLevelRegex.Replace(cleaned4, "").Trim();
		Assert.AreEqual("error  message", cleaned4);
	}
}
