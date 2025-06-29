using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleApp;
using Serilog.Events;

namespace Testing;

[TestClass]
public class LevelToggleTests
{
    [TestMethod]
    public void LoggingLevels_WhenModified_ShouldPersistChanges()
    {
        // Arrange
        var logLevels = new ApplicationLogLevels();
        var originalSystemLevel = logLevels.LoggingLevels["System"].MinimumLevel;
        
        // Act - Modify the logging level
        logLevels.LoggingLevels["System"].MinimumLevel = LogEventLevel.Debug;
        
        // Assert - The change should persist when accessing the property again
        Assert.AreEqual(LogEventLevel.Debug, logLevels.LoggingLevels["System"].MinimumLevel);
        Assert.AreNotEqual(originalSystemLevel, logLevels.LoggingLevels["System"].MinimumLevel);
    }
    
    [TestMethod]
    public void LoggingLevels_ShouldReturnSameInstancesOnMultipleAccess()
    {
        // Arrange
        var logLevels = new ApplicationLogLevels();
        
        // Act
        var firstAccess = logLevels.LoggingLevels;
        var secondAccess = logLevels.LoggingLevels;
        
        // Assert - Should return the same dictionary instance
        Assert.AreSame(firstAccess, secondAccess);
        
        // Assert - Should return the same LoggingLevelSwitch instances
        Assert.AreSame(firstAccess["System"], secondAccess["System"]);
        Assert.AreSame(firstAccess["Microsoft"], secondAccess["Microsoft"]);
        Assert.AreSame(firstAccess["SampleApp"], secondAccess["SampleApp"]);
        Assert.AreSame(firstAccess["SerilogViewer.SqlServer"], secondAccess["SerilogViewer.SqlServer"]);
    }
    
    [TestMethod]
    public void LoggingLevels_ShouldHaveCorrectInitialValues()
    {
        // Arrange & Act
        var logLevels = new ApplicationLogLevels();
        
        // Assert - Check initial values are as expected
        Assert.AreEqual(LogEventLevel.Warning, logLevels.LoggingLevels["System"].MinimumLevel);
        Assert.AreEqual(LogEventLevel.Warning, logLevels.LoggingLevels["Microsoft"].MinimumLevel);
        Assert.AreEqual(LogEventLevel.Debug, logLevels.LoggingLevels["SampleApp"].MinimumLevel);
        Assert.AreEqual(LogEventLevel.Debug, logLevels.LoggingLevels["SerilogViewer.SqlServer"].MinimumLevel);
    }
}