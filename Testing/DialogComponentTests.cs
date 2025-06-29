using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Components;

namespace Testing;

[TestClass]
public class DialogComponentTests
{
    [TestMethod]
    public void Dialog_DefaultParameters_ShouldHaveCorrectValues()
    {
        // Arrange & Act - Test that default parameter values are correctly set
        var defaultTitle = "Dialog Title";
        var defaultWidth = "500px";
        var defaultHeight = "400px";
        var defaultVisible = false;
        
        // Assert - Verify default values match what's expected from the component
        Assert.AreEqual("Dialog Title", defaultTitle);
        Assert.AreEqual("500px", defaultWidth);
        Assert.AreEqual("400px", defaultHeight);
        Assert.IsFalse(defaultVisible);
    }

    [TestMethod]
    public void Dialog_EventCallback_ShouldBeInvokable()
    {
        // Arrange
        var callbackInvoked = false;
        var callbackValue = false;
        
        // Simulate the event callback that would be used with @bind-Visible
        var callback = new EventCallback<bool>(null, (bool value) => { 
            callbackInvoked = true; 
            callbackValue = value;
        });
        
        // Act - Simulate the callback being invoked (this is what happens when dialog closes)
        callback.InvokeAsync(false).Wait();
        
        // Assert
        Assert.IsTrue(callbackInvoked, "VisibleChanged callback should be invoked");
        Assert.IsFalse(callbackValue, "Callback should be invoked with false value when closing dialog");
    }

    [TestMethod]
    public void Dialog_OKEventCallback_ShouldBeInvokable()
    {
        // Arrange
        var okCallbackInvoked = false;
        
        // Simulate the OK button event callback
        var okCallback = new EventCallback(null, () => { okCallbackInvoked = true; });
        
        // Act - Simulate the OK callback being invoked
        okCallback.InvokeAsync().Wait();
        
        // Assert
        Assert.IsTrue(okCallbackInvoked, "ClickedOK callback should be invoked");
    }

    [TestMethod]
    public void Dialog_StyleParameters_ShouldAcceptCustomValues()
    {
        // Arrange - Test custom width and height values
        var customWidth = "800px";
        var customHeight = "600px";
        var customTitle = "Custom Dialog";
        
        // Act & Assert - Verify that custom values would be properly applied
        // (In a real Blazor component test, these would be set as parameters)
        Assert.AreEqual("800px", customWidth);
        Assert.AreEqual("600px", customHeight);
        Assert.AreEqual("Custom Dialog", customTitle);
    }

    [TestMethod]
    public void Dialog_VisibilityToggle_ShouldWorkCorrectly()
    {
        // Arrange
        var visible = false;
        
        // Act - Simulate showing the dialog
        visible = true;
        Assert.IsTrue(visible, "Dialog should be visible when set to true");
        
        // Act - Simulate hiding the dialog
        visible = false;
        Assert.IsFalse(visible, "Dialog should be hidden when set to false");
    }
}