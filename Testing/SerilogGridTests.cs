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
        //Assert.AreEqual(0, component.Entries.Length);
    }
}