using SerilogViewer.SqlServer;

namespace Testing;

[TestClass]
public class ParseAgeTextTests
{
    [TestMethod]
    public void ParseAgeText_JustNow_ReturnsCorrectText()
    {
        // Arrange
        var ageMinutes = 0;

        // Act
        var result = SerilogSqlServerQuery.ParseAgeText(ageMinutes);

        // Assert
        Assert.AreEqual("just now", result);
    }

    [TestMethod]
    public void ParseAgeText_LessThanOneMinute_ReturnsJustNow()
    {
        // Arrange
        var ageMinutes = 0;

        // Act
        var result = SerilogSqlServerQuery.ParseAgeText(ageMinutes);

        // Assert
        Assert.AreEqual("just now", result);
    }

    [TestMethod]
    public void ParseAgeText_FiveMinutes_ReturnsCorrectFormat()
    {
        // Arrange
        var ageMinutes = 5;

        // Act
        var result = SerilogSqlServerQuery.ParseAgeText(ageMinutes);

        // Assert
        Assert.AreEqual("5m", result);
    }

    [TestMethod]
    public void ParseAgeText_OneHour_ReturnsCorrectFormat()
    {
        // Arrange
        var ageMinutes = 60;

        // Act
        var result = SerilogSqlServerQuery.ParseAgeText(ageMinutes);

        // Assert
        Assert.AreEqual("1h", result);
    }

    [TestMethod]
    public void ParseAgeText_OneHourAndMinutes_ReturnsCorrectFormat()
    {
        // Arrange
        var ageMinutes = 65; // 1 hour and 5 minutes

        // Act
        var result = SerilogSqlServerQuery.ParseAgeText(ageMinutes);

        // Assert
        Assert.AreEqual("1h, 5m", result);
    }

    [TestMethod]
    public void ParseAgeText_OneDay_DoesNotShowMinutes()
    {
        // Arrange
        var ageMinutes = 1440 + 65; // 1 day, 1 hour, 5 minutes

        // Act
        var result = SerilogSqlServerQuery.ParseAgeText(ageMinutes);

        // Assert
        Assert.AreEqual("1d, 1h", result); // Minutes should not be shown for days
    }

    [TestMethod]
    public void ParseAgeText_MultipleDays_ReturnsCorrectFormat()
    {
        // Arrange
        var ageMinutes = 2880 + 120 + 30; // 2 days, 2 hours, 30 minutes

        // Act
        var result = SerilogSqlServerQuery.ParseAgeText(ageMinutes);

        // Assert
        Assert.AreEqual("2d, 2h", result); // Minutes should not be shown for days
    }
}