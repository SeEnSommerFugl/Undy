using Undy.Data.Repository;

namespace UndyTest.Features.WholesaleOrders.Repositories;

[TestClass]
public sealed class WholesaleOrderRepositoriesSmokeTests
{
    [TestMethod]
    public void WholesaleOrderDBRepository_CanBeConstructed()
    {
        // Arrange

        // Act
        var repo = new WholesaleOrderDBRepository();

        // Assert
        Assert.IsNotNull(repo);
        Assert.IsNotNull(repo.Items);
    }

    [TestMethod]
    public void WholesaleOrderLineDBRepository_CanBeConstructed()
    {
        // Arrange

        // Act
        var repo = new WholesaleOrderLineDBRepository();

        // Assert
        Assert.IsNotNull(repo);
        Assert.IsNotNull(repo.Items);
    }

    [TestMethod]
    public async Task WholesaleOrderLineDBRepository_ProcessReceiptLinesAsync_WithEmptyInput_DoesNotThrow()
    {
        // Arrange
        var repo = new WholesaleOrderLineDBRepository();

        // Act
        await repo.ProcessReceiptLinesAsync(Array.Empty<(Guid WholesaleOrderID, Guid ProductID, int ReceiveQuantity)>());

        // Assert
        Assert.IsTrue(true);
    }
}
