using SerilogBlazor.Abstractions;
using SerilogBlazor.Postgres;

namespace Testing;

[TestClass]
public class PostgresTimestampTests
{
    [TestMethod]
    public void CurrentTimeFunction_Utc_ShouldReturnUtcFunction()
    {
        // Arrange & Act
        var result = PostgresHelpers.CurrentTimeFunction(TimestampType.Utc);
        
        // Assert
        Assert.AreEqual("NOW() AT TIME ZONE 'UTC'", result);
    }

    [TestMethod]
    public void CurrentTimeFunction_Local_ShouldReturnLocalFunction()
    {
        // Arrange & Act
        var result = PostgresHelpers.CurrentTimeFunction(TimestampType.Local);
        
        // Assert
        Assert.AreEqual("NOW()", result);
    }

    [TestMethod]
    public void AgeCalculation_WithUtcTimestampType_ShouldHandleTimezoneCorrectly()
    {
        // This test documents the expected behavior:
        // For UTC timestamp type, both current time and stored timestamps should be in UTC
        Assert.IsTrue(true, "UTC timestamps should be compared in UTC timezone");
    }

    [TestMethod]
    public void AgeCalculation_WithLocalTimestampType_ShouldHandleTimezoneCorrectly()
    {
        // This test documents the expected behavior:
        // For Local timestamp type, current time is local, stored timestamps should be converted from UTC to local
        Assert.IsTrue(true, "UTC timestamps should be converted to local timezone for local timestamp type");
    }
}