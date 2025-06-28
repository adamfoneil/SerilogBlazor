using SerilogViewer.Abstractions;

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
}
