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

	public void ExceptionText()
	{
		var input = "!NullReferenceException \"dandy warhols\"";
		var output = SerilogQuery.Criteria.ParseExpression(input);
		Assert.AreEqual("NullReferenceException", output.Exception);
		Assert.AreEqual("dandy warhols", output.Message);
	}
}
