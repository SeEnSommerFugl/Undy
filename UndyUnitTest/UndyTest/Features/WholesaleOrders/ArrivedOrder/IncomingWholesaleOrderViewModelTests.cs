using Undy.Features.ViewModel;
using Undy.Models;
using UndyTest.TestDoubles;

namespace UndyTest.Features.WholesaleOrders.ArrivedOrder;

[TestClass]
public sealed class IncomingWholesaleOrderViewModelTests
{
    [TestMethod]
    public void IsFullyReceived_WhenSetTrue_SetsAllSelectedOrderLinesQuantityReceivedToQuantity()
    {
        // Arrange
        var wholesaleRepo = new FakeBaseRepository<WholesaleOrder, Guid>(x => x.WholesaleOrderID);
        var productRepo = new FakeBaseRepository<Product, Guid>(x => x.ProductID);
        var lineRepo = new FakeWholesaleOrderLineDBRepository();

        var vm = new IncomingWholesaleOrderViewModel(wholesaleRepo, productRepo, lineRepo);

        vm.SelectedOrderLines.Add(new WholesaleOrderLine
        {
            WholesaleOrderID = Guid.NewGuid(),
            ProductID = Guid.NewGuid(),
            Quantity = 10,
            QuantityReceived = 1
        });
        vm.SelectedOrderLines.Add(new WholesaleOrderLine
        {
            WholesaleOrderID = Guid.NewGuid(),
            ProductID = Guid.NewGuid(),
            Quantity = 5,
            QuantityReceived = 0
        });

        // Act
        vm.IsFullyReceived = true;

        // Assert
        Assert.IsTrue(vm.SelectedOrderLines.All(l => l.QuantityReceived == l.Quantity));
    }

    [TestMethod]
    public async Task SelectedWholesaleOrder_WhenSet_LoadsLinesAndKeepsLoadedReceivedQuantity()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var wholesaleRepo = new FakeBaseRepository<WholesaleOrder, Guid>(x => x.WholesaleOrderID);
        var productRepo = new FakeBaseRepository<Product, Guid>(x => x.ProductID);
        var lineRepo = new FakeWholesaleOrderLineDBRepository();

        var lines = new List<WholesaleOrderLine>
        {
            new()
            {
                WholesaleOrderID = orderId,
                ProductID = Guid.NewGuid(),
                Quantity = 10,
                QuantityReceived = 2
            }
        };

        lineRepo.LinesByOrderId = id => id == orderId ? lines : new();

        var vm = new IncomingWholesaleOrderViewModel(wholesaleRepo, productRepo, lineRepo);

        // Act
        vm.SelectedWholesaleOrder = new WholesaleOrder { WholesaleOrderID = orderId, WholesaleOrderNumber = 1 };
        await Task.Delay(50);

        // Assert
        Assert.AreEqual(1, vm.SelectedOrderLines.Count);
        Assert.AreEqual(2, vm.SelectedOrderLines[0].QuantityReceived);
    }

    [TestMethod]
    public void ConfirmOrderCommand_CanExecute_False_WhenNoOrderSelected()
    {
        // Arrange
        var wholesaleRepo = new FakeBaseRepository<WholesaleOrder, Guid>(x => x.WholesaleOrderID);
        var productRepo = new FakeBaseRepository<Product, Guid>(x => x.ProductID);
        var lineRepo = new FakeWholesaleOrderLineDBRepository();

        var vm = new IncomingWholesaleOrderViewModel(wholesaleRepo, productRepo, lineRepo);

        // Act
        var can = vm.ConfirmOrderCommand.CanExecute(null);

        // Assert
        Assert.IsFalse(can);
    }

    [TestMethod]
    public async Task ConfirmOrderCommand_CanExecute_True_WhenAtLeastOneLineHasPositiveDelta()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var wholesaleRepo = new FakeBaseRepository<WholesaleOrder, Guid>(x => x.WholesaleOrderID);
        var productRepo = new FakeBaseRepository<Product, Guid>(x => x.ProductID);
        var lineRepo = new FakeWholesaleOrderLineDBRepository();

        var line = new WholesaleOrderLine
        {
            WholesaleOrderID = orderId,
            ProductID = Guid.NewGuid(),
            Quantity = 10,
            QuantityReceived = 2
        };

        lineRepo.LinesByOrderId = id => id == orderId ? [line] : new();

        var vm = new IncomingWholesaleOrderViewModel(wholesaleRepo, productRepo, lineRepo);

        vm.SelectedWholesaleOrder = new WholesaleOrder { WholesaleOrderID = orderId, WholesaleOrderNumber = 1 };
        await Task.Delay(50);

        // Act
        vm.SelectedOrderLines[0].QuantityReceived = 3;
        var can = vm.ConfirmOrderCommand.CanExecute(null);

        // Assert
        Assert.IsTrue(can);
    }

    [TestMethod]
    public async Task ConfirmReceiptAsync_WhenDeltaNegative_DoesNotCallProcessReceiptLines()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var wholesaleRepo = new FakeBaseRepository<WholesaleOrder, Guid>(x => x.WholesaleOrderID);
        var productRepo = new FakeBaseRepository<Product, Guid>(x => x.ProductID);
        var lineRepo = new FakeWholesaleOrderLineDBRepository();

        var line = new WholesaleOrderLine
        {
            WholesaleOrderID = orderId,
            ProductID = Guid.NewGuid(),
            ProductNumber = "P1",
            ProductName = "Prod",
            Quantity = 10,
            QuantityReceived = 5
        };

        lineRepo.LinesByOrderId = id => id == orderId ? [line] : new();

        var vm = new IncomingWholesaleOrderViewModel(wholesaleRepo, productRepo, lineRepo)
        {
            SelectedWholesaleOrder = new WholesaleOrder { WholesaleOrderID = orderId, WholesaleOrderNumber = 1 }
        };

        await Task.Delay(50);

        vm.SelectedOrderLines[0].QuantityReceived = 4;

        // Act
        vm.ConfirmOrderCommand.Execute(null);
        await Task.Delay(50);

        // Assert
        Assert.AreEqual(0, lineRepo.LastReceipts.Count);
    }

    [TestMethod]
    public async Task ConfirmReceiptAsync_WhenPositiveDelta_CallsProcessReceiptLinesWithDeltaOnly()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var wo = new WholesaleOrder { WholesaleOrderID = orderId, WholesaleOrderNumber = 1 };

        var wholesaleRepo = new FakeBaseRepository<WholesaleOrder, Guid>(x => x.WholesaleOrderID);
        wholesaleRepo.Items.Add(wo);

        var productRepo = new FakeBaseRepository<Product, Guid>(x => x.ProductID);
        var lineRepo = new FakeWholesaleOrderLineDBRepository();

        var line = new WholesaleOrderLine
        {
            WholesaleOrderID = orderId,
            ProductID = Guid.NewGuid(),
            Quantity = 10,
            QuantityReceived = 2
        };

        lineRepo.LinesByOrderId = id => id == orderId ? [line] : new();

        var vm = new IncomingWholesaleOrderViewModel(wholesaleRepo, productRepo, lineRepo)
        {
            SelectedWholesaleOrder = wo
        };

        await Task.Delay(50);

        vm.SelectedOrderLines[0].QuantityReceived = 5;

        // Act
        vm.ConfirmOrderCommand.Execute(null);
        await Task.Delay(50);

        // Assert
        Assert.AreEqual(1, lineRepo.LastReceipts.Count);
        Assert.AreEqual(orderId, lineRepo.LastReceipts[0].WholesaleOrderID);
        Assert.AreEqual(line.ProductID, lineRepo.LastReceipts[0].ProductID);
        Assert.AreEqual(3, lineRepo.LastReceipts[0].ReceiveQuantity);
    }
}
