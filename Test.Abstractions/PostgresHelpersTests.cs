using SerilogBlazor.Postgres;

namespace Testing;

[TestClass]
public class PostgresHelpersTests
{
    [TestMethod]
    public void LevelIntToString_ValidLevels_ReturnsCorrectStrings()
    {
        // Test all standard Serilog levels
        Assert.AreEqual("Verbose", PostgresHelpers.LevelIntToString(0));
        Assert.AreEqual("Debug", PostgresHelpers.LevelIntToString(1));
        Assert.AreEqual("Information", PostgresHelpers.LevelIntToString(2));
        Assert.AreEqual("Warning", PostgresHelpers.LevelIntToString(3));
        Assert.AreEqual("Error", PostgresHelpers.LevelIntToString(4));
        Assert.AreEqual("Fatal", PostgresHelpers.LevelIntToString(5));
    }

    [TestMethod]
    public void LevelIntToString_InvalidLevel_ReturnsNumberAsString()
    {
        // Test with invalid level number
        Assert.AreEqual("99", PostgresHelpers.LevelIntToString(99));
        Assert.AreEqual("-1", PostgresHelpers.LevelIntToString(-1));
    }

    [TestMethod]
    public void LevelStringToInt_ValidLevels_ReturnsCorrectInts()
    {
        // Test all standard Serilog levels
        Assert.AreEqual(0, PostgresHelpers.LevelStringToInt("Verbose"));
        Assert.AreEqual(1, PostgresHelpers.LevelStringToInt("Debug"));
        Assert.AreEqual(2, PostgresHelpers.LevelStringToInt("Information"));
        Assert.AreEqual(3, PostgresHelpers.LevelStringToInt("Warning"));
        Assert.AreEqual(4, PostgresHelpers.LevelStringToInt("Error"));
        Assert.AreEqual(5, PostgresHelpers.LevelStringToInt("Fatal"));
    }

    [TestMethod]
    public void LevelStringToInt_InvalidLevel_ThrowsArgumentException()
    {
        // Test with invalid level string
        Assert.ThrowsException<ArgumentException>(() => PostgresHelpers.LevelStringToInt("InvalidLevel"));
        Assert.ThrowsException<ArgumentException>(() => PostgresHelpers.LevelStringToInt(""));
        Assert.ThrowsException<ArgumentException>(() => PostgresHelpers.LevelStringToInt("debug")); // case sensitive
    }

    [TestMethod]
    public void LevelConversion_RoundTrip_PreservesValue()
    {
        // Test round-trip conversion for all valid levels
        var levels = new[] { "Verbose", "Debug", "Information", "Warning", "Error", "Fatal" };
        
        foreach (var level in levels)
        {
            var intValue = PostgresHelpers.LevelStringToInt(level);
            var stringValue = PostgresHelpers.LevelIntToString(intValue);
            Assert.AreEqual(level, stringValue);
        }
    }

    [TestMethod]
    public void CurrentTimeFunction_Utc_ReturnsPostgresUtcFunction()
    {
        // Arrange & Act
        var result = PostgresHelpers.CurrentTimeFunction(SerilogBlazor.Abstractions.TimestampType.Utc);
        
        // Assert
        Assert.AreEqual("NOW() AT TIME ZONE 'UTC'", result);
    }

    [TestMethod]
    public void CurrentTimeFunction_Local_ReturnsPostgresLocalFunction()
    {
        // Arrange & Act
        var result = PostgresHelpers.CurrentTimeFunction(SerilogBlazor.Abstractions.TimestampType.Local);
        
        // Assert
        Assert.AreEqual("NOW()", result);
    }
}