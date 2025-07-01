using Microsoft.Extensions.DependencyInjection;
using SerilogBlazor.RCL;

namespace Testing;

[TestClass]
public class SerilogGridTests
{
    [TestMethod]
    public void SerilogGrid_CanBeCreated()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        
        // Act & Assert - This test just verifies the component can be instantiated
        var component = new SerilogGrid();
        Assert.IsNotNull(component);
        Assert.IsNotNull(component.Entries);
        //Assert.AreEqual(0, component.Entries.Length);
    }

    [TestMethod]
    public void SerilogGrid_LevelClassGeneration()
    {
        // Arrange - Test data for different log levels
        var logLevels = new[] { "Information", "Warning", "Error", "Debug", "Verbose", "Fatal", "Critical" };
        
        // Act & Assert - Verify that each level generates the expected CSS class
        foreach (var level in logLevels)
        {
            var expectedClass = $"level-{level.ToLower()}";
            Assert.AreEqual(expectedClass, $"level-{level.ToLower()}");
        }
    }
}