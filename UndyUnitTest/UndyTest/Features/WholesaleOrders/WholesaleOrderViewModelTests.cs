using Undy.Features.ViewModel;
using Undy.Models;
using UndyTest.TestDoubles;

namespace UndyTest.Features.WholesaleOrders;

[TestClass]
public sealed class WholesaleOrderViewModelTests
{
    [TestMethod]
    public void ProductNumberSearch_WhenChanged_RefreshesView()
    {
        // Arrange
        var wholesaleRepo = new FakeBaseRepository<WholesaleOrder, Guid>(x => x.WholesaleOrderID);
        var wholesaleDisplayRepo = new FakeBaseRepository<WholesaleOrderDisplay, Guid>(x => x.WholesaleOrderID);
        var productRepo = new FakeBaseRepository<Product, Guid>(x => x.ProductID);
        var lineRepo = new FakeWholesaleOrderLineDBRepository();

        wholesaleDisplayRepo.Items.Add(new WholesaleOrderDisplay { WholesaleOrderID = Guid.NewGuid(), ProductNumber = "100", ProductName = "A" });
        wholesaleDisplayRepo.Items.Add(new WholesaleOrderDisplay { WholesaleOrderID = Guid.NewGuid(), ProductNumber = "200", ProductName = "B" });

        var vm = new WholesaleOrderViewModel(wholesaleRepo, wholesaleDisplayRepo, productRepo, lineRepo);

        // Act
        vm.ProductNumberSearch = 100;

        // Assert
        Assert.AreEqual(1, vm.WholesaleView.Cast<object>().Count());
    }

    [TestMethod]
    public void ProductNameSearch_WhenChanged_RefreshesView()
    {
        // Arrange
        var wholesaleRepo = new FakeBaseRepository<WholesaleOrder, Guid>(x => x.WholesaleOrderID);
        var wholesaleDisplayRepo = new FakeBaseRepository<WholesaleOrderDisplay, Guid>(x => x.WholesaleOrderID);
        var productRepo = new FakeBaseRepository<Product, Guid>(x => x.ProductID);
        var lineRepo = new FakeWholesaleOrderLineDBRepository();

        wholesaleDisplayRepo.Items.Add(new WholesaleOrderDisplay { WholesaleOrderID = Guid.NewGuid(), ProductNumber = "100", ProductName = "Blue Shirt" });
        wholesaleDisplayRepo.Items.Add(new WholesaleOrderDisplay { WholesaleOrderID = Guid.NewGuid(), ProductNumber = "200", ProductName = "Red Pants" });

        var vm = new WholesaleOrderViewModel(wholesaleRepo, wholesaleDisplayRepo, productRepo, lineRepo);

        // Act
        vm.ProductNameSearch = "blue";

        // Assert
        Assert.AreEqual(1, vm.WholesaleView.Cast<object>().Count());
    }

    [TestMethod]
    public void AddProductCommand_CanExecute_RequiresSelectedProductAndPositiveQuantity()
    {
        // Arrange
        var wholesaleRepo = new FakeBaseRepository<WholesaleOrder, Guid>(x => x.WholesaleOrderID);
        var wholesaleDisplayRepo = new FakeBaseRepository<WholesaleOrderDisplay, Guid>(x => x.WholesaleOrderID);
        var productRepo = new FakeBaseRepository<Product, Guid>(x => x.ProductID);
        var lineRepo = new FakeWholesaleOrderLineDBRepository();

        var vm = new WholesaleOrderViewModel(wholesaleRepo, wholesaleDisplayRepo, productRepo, lineRepo);

        // Act
        vm.SelectedProduct = null;
        vm.Quantity = 5;
        var can1 = vm.AddProductCommand.CanExecute(null);

        vm.SelectedProduct = new Product { ProductID = Guid.NewGuid(), ProductName = "A", Price = 10 };
        vm.Quantity = 0;
        var can2 = vm.AddProductCommand.CanExecute(null);

        vm.Quantity = 3;
        var can3 = vm.AddProductCommand.CanExecute(null);

        // Assert
        Assert.IsFalse(can1);
        Assert.IsFalse(can2);
        Assert.IsTrue(can3);
    }

    [TestMethod]
    public void AddProduct_WhenNewProduct_AddsLineAndResetsQuantity()
    {
        // Arrange
        var wholesaleRepo = new FakeBaseRepository<WholesaleOrder, Guid>(x => x.WholesaleOrderID);
        var wholesaleDisplayRepo = new FakeBaseRepository<WholesaleOrderDisplay, Guid>(x => x.WholesaleOrderID);
        var productRepo = new FakeBaseRepository<Product, Guid>(x => x.ProductID);
        var lineRepo = new FakeWholesaleOrderLineDBRepository();

        var vm = new WholesaleOrderViewModel(wholesaleRepo, wholesaleDisplayRepo, productRepo, lineRepo)
        {
            SelectedProduct = new Product { ProductID = Guid.NewGuid(), ProductName = "A", Price = 10 },
            Quantity = 2
        };

        // Act
        vm.AddProductCommand.Execute(null);

        // Assert
        Assert.AreEqual(1, vm.WholesaleOrderLines.Count);
        Assert.AreEqual(0, vm.Quantity);
        Assert.AreEqual("A", vm.WholesaleOrderLines[0].ProductName);
        Assert.AreEqual(2, vm.WholesaleOrderLines[0].Quantity);
        Assert.AreEqual(10m, vm.WholesaleOrderLines[0].UnitPrice);
    }

    [TestMethod]
    public void AddProduct_WhenExistingProduct_IncrementsQuantity()
    {
        // Arrange
        var pid = Guid.NewGuid();
        var wholesaleRepo = new FakeBaseRepository<WholesaleOrder, Guid>(x => x.WholesaleOrderID);
        var wholesaleDisplayRepo = new FakeBaseRepository<WholesaleOrderDisplay, Guid>(x => x.WholesaleOrderID);
        var productRepo = new FakeBaseRepository<Product, Guid>(x => x.ProductID);
        var lineRepo = new FakeWholesaleOrderLineDBRepository();

        var vm = new WholesaleOrderViewModel(wholesaleRepo, wholesaleDisplayRepo, productRepo, lineRepo);
        vm.WholesaleOrderLines.Add(new WholesaleOrderViewModel.WholesaleOrderLineEntryViewModel
        {
            ProductID = pid,
            ProductName = "A",
            UnitPrice = 10,
            Quantity = 2,
            QuantityReceived = 0
        });

        vm.SelectedProduct = new Product { ProductID = pid, ProductName = "A", Price = 10 };
        vm.Quantity = 3;

        // Act
        vm.AddProductCommand.Execute(null);

        // Assert
        Assert.AreEqual(1, vm.WholesaleOrderLines.Count);
        Assert.AreEqual(5, vm.WholesaleOrderLines[0].Quantity);
    }

    [TestMethod]
    public void RemoveWholesaleOrderLineCommand_RemovesLine()
    {
        // Arrange
        var wholesaleRepo = new FakeBaseRepository<WholesaleOrder, Guid>(x => x.WholesaleOrderID);
        var wholesaleDisplayRepo = new FakeBaseRepository<WholesaleOrderDisplay, Guid>(x => x.WholesaleOrderID);
        var productRepo = new FakeBaseRepository<Product, Guid>(x => x.ProductID);
        var lineRepo = new FakeWholesaleOrderLineDBRepository();

        var vm = new WholesaleOrderViewModel(wholesaleRepo, wholesaleDisplayRepo, productRepo, lineRepo);
        var line = new WholesaleOrderViewModel.WholesaleOrderLineEntryViewModel { ProductID = Guid.NewGuid(), ProductName = "A", Quantity = 1, UnitPrice = 1 };
        vm.WholesaleOrderLines.Add(line);

        // Act
        vm.RemoveWholesaleOrderLineCommand.Execute(line);

        // Assert
        Assert.AreEqual(0, vm.WholesaleOrderLines.Count);
    }

    [TestMethod]
    public void ConfirmCommand_WhenNoLines_SetsErrorFeedback()
    {
        // Arrange
        var wholesaleRepo = new FakeBaseRepository<WholesaleOrder, Guid>(x => x.WholesaleOrderID);
        var wholesaleDisplayRepo = new FakeBaseRepository<WholesaleOrderDisplay, Guid>(x => x.WholesaleOrderID);
        var productRepo = new FakeBaseRepository<Product, Guid>(x => x.ProductID);
        var lineRepo = new FakeWholesaleOrderLineDBRepository();

        var vm = new WholesaleOrderViewModel(wholesaleRepo, wholesaleDisplayRepo, productRepo, lineRepo);

        // Act
        vm.ConfirmCommand.Execute(null);

        // Assert
        Assert.IsTrue(vm.IsConfirmError);
        Assert.IsNotNull(vm.ConfirmFeedback);
    }

    [TestMethod]
    public async Task ConfirmAsync_WhenHasLines_CallsRepos_SetsSuccessFeedback_AndClearsForm()
    {
        // Arrange
        var wholesaleRepo = new FakeBaseRepository<WholesaleOrder, Guid>(x => x.WholesaleOrderID);

        var wholesaleDisplayRepo = new FakeBaseRepository<WholesaleOrderDisplay, Guid>(x => x.WholesaleOrderID);
        var productRepo = new FakeBaseRepository<Product, Guid>(x => x.ProductID);

        var lineRepo = new FakeWholesaleOrderLineWriteRepository();

        var vm = new WholesaleOrderViewModel(wholesaleRepo, wholesaleDisplayRepo, productRepo, lineRepo)
        {
            ExpectedDeliveryDate = new DateTime(2030, 1, 2)
        };

        vm.WholesaleOrderLines.Add(new WholesaleOrderViewModel.WholesaleOrderLineEntryViewModel
        {
            ProductID = Guid.NewGuid(),
            ProductName = "P",
            Quantity = 3,
            UnitPrice = 12.50m,
            QuantityReceived = 0
        });

        vm.SelectedProduct = new Product { ProductID = Guid.NewGuid(), ProductName = "X", Price = 1m };
        vm.Quantity = 99;

        // Act
        await vm.ConfirmAsync();

        // Assert
        Assert.IsFalse(vm.IsConfirmError);
        Assert.IsNotNull(vm.ConfirmFeedback);

        Assert.AreEqual(1, wholesaleRepo.Items.Count);
        Assert.AreEqual(1, lineRepo.AddedLines.Count);
        Assert.AreEqual(wholesaleRepo.Items[0].WholesaleOrderID, lineRepo.AddedLines[0].WholesaleOrderID);
        Assert.AreEqual(3, lineRepo.AddedLines[0].Quantity);
        Assert.AreEqual(12.50m, lineRepo.AddedLines[0].UnitPrice);

        Assert.AreEqual(0, vm.WholesaleOrderLines.Count);
        Assert.IsNull(vm.SelectedProduct);
        Assert.AreEqual(0, vm.Quantity);
        Assert.AreEqual(DateTime.Today, vm.ExpectedDeliveryDate);
    }

    [TestMethod]
    public async Task ConfirmAsync_WhenWholesaleOrderRepoThrows_SetsErrorFeedback()
    {
        // Arrange
        var ex = new InvalidOperationException("boom");
        var wholesaleRepo = new ThrowingBaseRepository<WholesaleOrder, Guid>(ex);
        var wholesaleDisplayRepo = new FakeBaseRepository<WholesaleOrderDisplay, Guid>(x => x.WholesaleOrderID);
        var productRepo = new FakeBaseRepository<Product, Guid>(x => x.ProductID);
        var lineRepo = new FakeWholesaleOrderLineWriteRepository();

        var vm = new WholesaleOrderViewModel(wholesaleRepo, wholesaleDisplayRepo, productRepo, lineRepo);
        vm.WholesaleOrderLines.Add(new WholesaleOrderViewModel.WholesaleOrderLineEntryViewModel
        {
            ProductID = Guid.NewGuid(),
            Quantity = 1,
            UnitPrice = 1m,
            QuantityReceived = 0
        });

        // Act
        await vm.ConfirmAsync();

        // Assert
        Assert.IsTrue(vm.IsConfirmError);
        Assert.IsNotNull(vm.ConfirmFeedback);
        StringAssert.Contains(vm.ConfirmFeedback, "boom");

        Assert.AreEqual(0, lineRepo.AddedLines.Count);
    }
}
