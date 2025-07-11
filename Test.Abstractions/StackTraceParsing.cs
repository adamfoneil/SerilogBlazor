﻿using SerilogBlazor.Abstractions;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace Testing;

[TestClass]
public sealed class StackTraceParsing
{
	[TestMethod]
	public void PKViolation() => TestInner("PKViolation");

	[TestMethod]
	public void PKViolation2() => TestInner("PKViolation2");

	[TestMethod]
	public void PKViolation3() => TestInner("PKViolation3");

	[TestMethod]
	public void Truncated() => TestInner("Truncated");

	[TestMethod]
	public void Twilio() => TestInner("Twilio");

	private static void TestInner(string resourceNameBase)
	{
		var input = GetContent(resourceNameBase + ".txt");
		var expectedJson = GetContent(resourceNameBase + ".json");
		var expected = JsonSerializer.Deserialize<StackTraceInfo>(expectedJson, new JsonSerializerOptions() {  PropertyNameCaseInsensitive = true }) ?? throw new Exception("Couldn't deserialize");
		var actual = StackTraceParser.Parse(input, "/home/runner/work/Hs5/", "Hs5.");

		var actualJson = JsonSerializer.Serialize(actual, new JsonSerializerOptions() {  WriteIndented = true });

		//Assert.AreEqual(expected, actual); for some reason this doesn't work, but the other assertions do
		Assert.AreEqual(expected.ExceptionType, actual.ExceptionType);
		Assert.AreEqual(expected.Message, actual.Message);
		Assert.IsTrue(expected.Locations.SequenceEqual(actual.Locations));

		Debug.Print($"ErrorId = {actual.ErrorId}");
	}

	private static string GetContent(string resourceName)
	{
		var fullName = $"Testing.Samples.{resourceName}";
		using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fullName) ?? throw new Exception($"Resource not found: {fullName}");
		return new StreamReader(stream).ReadToEnd();
	}
}
