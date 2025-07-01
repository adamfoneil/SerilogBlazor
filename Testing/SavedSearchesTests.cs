using SerilogBlazor.Abstractions.SavedSearches;

namespace Testing;

[TestClass]
public class SavedSearchesTests
{
    [TestMethod]
    public void SerilogSavedSearch_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var search = new SerilogSavedSearch
        {
            Id = 1,
            UserName = "testuser",
            SearchName = "Test Search",
            Expression = "test expression",
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        Assert.AreEqual(1, search.Id);
        Assert.AreEqual("testuser", search.UserName);
        Assert.AreEqual("Test Search", search.SearchName);
        Assert.AreEqual("test expression", search.Expression);
        Assert.IsTrue(search.CreatedAt <= DateTime.UtcNow);
        Assert.IsNull(search.UpdatedAt);
    }

    [TestMethod]
    public void SerilogSavedSearch_ShouldHaveDefaultCreatedAt()
    {
        // Arrange & Act
        var search = new SerilogSavedSearch();
        var timeBefore = DateTime.UtcNow.AddSeconds(-1);
        var timeAfter = DateTime.UtcNow.AddSeconds(1);

        // Assert
        Assert.IsTrue(search.CreatedAt >= timeBefore && search.CreatedAt <= timeAfter,
            "CreatedAt should be set to current UTC time by default");
    }

    [TestMethod]
    public void SerilogSavedSearch_UpdatedAt_ShouldBeNullByDefault()
    {
        // Arrange & Act
        var search = new SerilogSavedSearch();

        // Assert
        Assert.IsNull(search.UpdatedAt);
    }
}