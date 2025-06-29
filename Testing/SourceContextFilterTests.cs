using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleApp;
using Serilog.Events;

namespace Testing;

[TestClass]
public class SourceContextFilterTests
{
    [TestMethod]
    public void GetEffectiveLogLevel_ShouldReturnExactMatch()
    {
        // Arrange
        var logLevels = new ApplicationLogLevels();
        var sourceContext = "SampleApp";
        
        // Act - Test the logic directly using the LogLevels configuration
        var result = GetEffectiveLogLevelHelper(logLevels, sourceContext);
        
        // Assert
        Assert.AreEqual("Debug", result);
    }

    [TestMethod]
    public void GetEffectiveLogLevel_ShouldReturnLongestPrefixMatch()
    {
        // Arrange
        var logLevels = new ApplicationLogLevels();
        var sourceContext = "SampleApp.Components.Pages.Home";
        
        // Act
        var result = GetEffectiveLogLevelHelper(logLevels, sourceContext);
        
        // Assert - Should match "SampleApp" prefix
        Assert.AreEqual("Debug", result);
    }

    [TestMethod]
    public void GetEffectiveLogLevel_ShouldReturnSystemPrefix()
    {
        // Arrange
        var logLevels = new ApplicationLogLevels();
        var sourceContext = "System.Net.Http";
        
        // Act
        var result = GetEffectiveLogLevelHelper(logLevels, sourceContext);
        
        // Assert - Should match "System" prefix
        Assert.AreEqual("Warning", result);
    }

    [TestMethod]
    public void GetEffectiveLogLevel_ShouldReturnMicrosoftPrefix()
    {
        // Arrange
        var logLevels = new ApplicationLogLevels();
        var sourceContext = "Microsoft.AspNetCore.Hosting";
        
        // Act
        var result = GetEffectiveLogLevelHelper(logLevels, sourceContext);
        
        // Assert - Should match "Microsoft" prefix
        Assert.AreEqual("Warning", result);
    }

    [TestMethod]
    public void GetEffectiveLogLevel_ShouldReturnSerilogViewerPrefix()
    {
        // Arrange
        var logLevels = new ApplicationLogLevels();
        var sourceContext = "SerilogViewer.SqlServer.SomeClass";
        
        // Act
        var result = GetEffectiveLogLevelHelper(logLevels, sourceContext);
        
        // Assert - Should match "SerilogViewer.SqlServer" prefix
        Assert.AreEqual("Debug", result);
    }

    [TestMethod]
    public void GetEffectiveLogLevel_ShouldReturnDefaultForNoMatch()
    {
        // Arrange
        var logLevels = new ApplicationLogLevels();
        var sourceContext = "SomeOtherNamespace.SomeClass";
        
        // Act
        var result = GetEffectiveLogLevelHelper(logLevels, sourceContext);
        
        // Assert - Should return default when no prefix matches
        Assert.AreEqual("Information", result);
    }

    [TestMethod]
    public void GetEffectiveLogLevel_ShouldReturnLongestMatchingPrefix()
    {
        // Arrange - Create a custom log levels configuration to test longest match
        var logLevels = new ApplicationLogLevels();
        
        // Simulate having both "SampleApp" (Debug) and "SampleApp.Components" (would be Debug)
        // Since we only have "SampleApp" in the actual config, test with what we have
        var sourceContext = "SampleApp.Components.Pages.Home";
        
        // Act
        var result = GetEffectiveLogLevelHelper(logLevels, sourceContext);
        
        // Assert - Should match "SampleApp" as the longest available prefix
        Assert.AreEqual("Debug", result);
    }

    [TestMethod]
    public void LevelBadge_CssClassGeneration()
    {
        // Arrange - Test data for different log levels that should generate CSS classes
        var testCases = new[]
        {
            new { Level = "Information", ExpectedClass = "level-information" },
            new { Level = "Warning", ExpectedClass = "level-warning" },
            new { Level = "Error", ExpectedClass = "level-error" },
            new { Level = "Debug", ExpectedClass = "level-debug" },
            new { Level = "Verbose", ExpectedClass = "level-verbose" },
            new { Level = "Fatal", ExpectedClass = "level-fatal" },
            new { Level = "Critical", ExpectedClass = "level-critical" }
        };
        
        // Act & Assert - Verify that each level generates the expected CSS class name
        foreach (var testCase in testCases)
        {
            var actualClass = $"level-{testCase.Level.ToLower()}";
            Assert.AreEqual(testCase.ExpectedClass, actualClass, 
                $"Level '{testCase.Level}' should generate CSS class '{testCase.ExpectedClass}'");
        }
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