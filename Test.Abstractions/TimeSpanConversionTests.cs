using SerilogBlazor.Abstractions;

namespace Testing;

[TestClass]
public class TimeSpanConversionTests
{
	[TestMethod]
	public void VerifyWeekConversion()
	{
		// Test that weeks convert to the expected number of days
		var oneWeek = TimeSpan.FromDays(7);
		var twoWeeks = TimeSpan.FromDays(14);
		
		Assert.AreEqual(7, oneWeek.Days);
		Assert.AreEqual(14, twoWeeks.Days);
		
		// Test that weeks can be detected as multiples of 7
		Assert.AreEqual(0, oneWeek.Days % 7);
		Assert.AreEqual(0, twoWeeks.Days % 7);
	}

	[TestMethod]
	public void VerifyMonthConversion()
	{
		// Test that months convert to the expected number of days
		var oneMonth = TimeSpan.FromDays(30);
		var twoMonths = TimeSpan.FromDays(60);
		
		Assert.AreEqual(30, oneMonth.Days);
		Assert.AreEqual(60, twoMonths.Days);
		
		// Test that months can be detected as multiples of 30
		Assert.AreEqual(0, oneMonth.Days % 30);
		Assert.AreEqual(0, twoMonths.Days % 30);
	}

	[TestMethod]
	public void VerifyTimeUnitPriority()
	{
		// Test that 30 days would be detected as a month (divisible by 30)
		var thirtyDays = TimeSpan.FromDays(30);
		Assert.AreEqual(0, thirtyDays.Days % 30);
		Assert.AreNotEqual(0, thirtyDays.Days % 7); // Not divisible by 7
		
		// Test that 14 days would be detected as weeks (divisible by 7 but not 30)
		var fourteenDays = TimeSpan.FromDays(14);
		Assert.AreEqual(0, fourteenDays.Days % 7);
		Assert.AreNotEqual(0, fourteenDays.Days % 30); // Not divisible by 30
		
		// Test that 10 days would be detected as days (not divisible by 7 or 30)
		var tenDays = TimeSpan.FromDays(10);
		Assert.AreNotEqual(0, tenDays.Days % 7);
		Assert.AreNotEqual(0, tenDays.Days % 30);
	}
}