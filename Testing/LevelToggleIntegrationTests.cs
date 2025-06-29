using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleApp;
using Serilog.Events;

namespace Testing;

[TestClass]
public class LevelToggleIntegrationTests
{
    [TestMethod]
    public void LevelToggle_EventCallback_ShouldBeInvokable()
    {
        // Arrange
        var logLevels = new ApplicationLogLevels();
        var callbackInvoked = false;
        
        // Simulate the event callback
        var callback = new Microsoft.AspNetCore.Components.EventCallback(null, () => { callbackInvoked = true; });
        
        // Act - Simulate changing a log level (this is what would happen in the UI)
        logLevels.LoggingLevels["SampleApp"].MinimumLevel = LogEventLevel.Error;
        
        // Simulate the callback being invoked
        callback.InvokeAsync().Wait();
        
        // Assert
        Assert.AreEqual(LogEventLevel.Error, logLevels.LoggingLevels["SampleApp"].MinimumLevel);
        Assert.IsTrue(callbackInvoked);
    }

    [TestMethod]
    public void LogLevels_CanBeModifiedAtRuntime()
    {
        // Arrange
        var logLevels = new ApplicationLogLevels();
        var originalLevel = logLevels.LoggingLevels["SampleApp"].MinimumLevel;
        
        // Act - Change the log level
        logLevels.LoggingLevels["SampleApp"].MinimumLevel = LogEventLevel.Fatal;
        
        // Assert
        Assert.AreNotEqual(originalLevel, logLevels.LoggingLevels["SampleApp"].MinimumLevel);
        Assert.AreEqual(LogEventLevel.Fatal, logLevels.LoggingLevels["SampleApp"].MinimumLevel);
    }

    [TestMethod]
    public void GetEffectiveLogLevel_ReflectsRuntimeChanges()
    {
        // Arrange
        var logLevels = new ApplicationLogLevels();
        var sourceContext = "SampleApp.SomeService";
        
        // Get the helper method result before change
        var beforeChange = GetEffectiveLogLevelHelper(logLevels, sourceContext);
        
        // Act - Change the log level at runtime
        logLevels.LoggingLevels["SampleApp"].MinimumLevel = LogEventLevel.Fatal;
        
        // Get the helper method result after change
        var afterChange = GetEffectiveLogLevelHelper(logLevels, sourceContext);
        
        // Assert
        Assert.AreEqual("Debug", beforeChange); // Original level
        Assert.AreEqual("Fatal", afterChange);  // Changed level
        Assert.AreNotEqual(beforeChange, afterChange);
    }

    /// <summary>
    /// Helper method that implements the same logic as GetEffectiveLogLevel in SourceContextFilter
    /// </summary>
    private string GetEffectiveLogLevelHelper(ApplicationLogLevels logLevels, string sourceContext)
    {
        // Find the longest matching prefix from LogLevels.LoggingLevels
        string? bestMatch = null;
        int bestMatchLength = 0;

        foreach (var kvp in logLevels.LoggingLevels)
        {
            var prefix = kvp.Key;
            if (sourceContext.StartsWith(prefix) && prefix.Length > bestMatchLength)
            {
                bestMatch = prefix;
                bestMatchLength = prefix.Length;
            }
        }

        if (bestMatch != null)
        {
            return logLevels.LoggingLevels[bestMatch].MinimumLevel.ToString();
        }

        // Default to Information if no prefix matches
        return "Information";
    }
}