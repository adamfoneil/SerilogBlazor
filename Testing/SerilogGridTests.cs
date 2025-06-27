using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using SerilogViewer.Abstractions;
using SerilogViewer.RCL;

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
        Assert.AreEqual(0, component.Entries.Length);
    }

    [TestMethod]
    public void SerilogGrid_CanHandleEntries()
    {
        // Arrange
        var component = new SerilogGrid();
        var entries = new SerilogEntry[]
        {
            new SerilogEntry(
                Id: 1,
                Timestamp: DateTime.Now,
                SourceContext: "Test.Context",
                RequestId: "req-123",
                Level: "Information",
                MessageTemplate: "Test message {Value}",
                Message: "Test message 42",
                Exception: null,
                Properties: new Dictionary<string, object> { {"Value", 42} }
            )
        };

        // Act
        component.Entries = entries;

        // Assert
        Assert.AreEqual(1, component.Entries.Length);
        Assert.AreEqual("Test.Context", component.Entries[0].SourceContext);
        Assert.AreEqual("Information", component.Entries[0].Level);
        Assert.IsTrue(component.Entries[0].Properties.ContainsKey("Value"));
    }
}