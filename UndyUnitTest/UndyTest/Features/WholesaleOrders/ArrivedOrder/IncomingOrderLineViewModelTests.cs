using Undy.Features.ViewModel;

namespace UndyTest.Features.WholesaleOrders.ArrivedOrder;

[TestClass]
public sealed class IncomingOrderLineViewModelTests
{
    [TestMethod]
    public void ReceivedQuantity_WhenChanged_RecalculatesDifference_AndRaisesDifferencePropertyChanged()
    {
        // Arrange
        var vm = new IncomingOrderLineViewModel
        {
            OrderedQuantity = 10,
            AlreadyReceived = 2,
            ReceivedQuantity = 0
        };

        var changed = new List<string?>();
        vm.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        // Act
        vm.ReceivedQuantity = 3;

        // Assert
        Assert.AreEqual(5, vm.Difference);
        CollectionAssert.Contains(changed, nameof(IncomingOrderLineViewModel.ReceivedQuantity));
        CollectionAssert.Contains(changed, nameof(IncomingOrderLineViewModel.Difference));
    }

    [TestMethod]
    public void ReceivedQuantity_WhenSetToSameValue_DoesNotRaiseDifferenceChange_AndDoesNotRecalculate()
    {
        // Arrange
        var vm = new IncomingOrderLineViewModel
        {
            OrderedQuantity = 10,
            AlreadyReceived = 2,
            ReceivedQuantity = 3,
            Difference = 123
        };

        var changed = new List<string?>();
        vm.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        // Act
        vm.ReceivedQuantity = 3;

        // Assert
        Assert.AreEqual(123, vm.Difference);
        Assert.IsFalse(changed.Contains(nameof(IncomingOrderLineViewModel.Difference)));
    }

    [TestMethod]
    public void Difference_Computation_CoversAllBranches()
    {
        // Arrange
        var vm = new IncomingOrderLineViewModel
        {
            OrderedQuantity = 10,
            AlreadyReceived = 0,
            ReceivedQuantity = 0
        };

        // Act + Assert
        vm.ReceivedQuantity = 4;
        Assert.AreEqual(6, vm.Difference);

        vm.AlreadyReceived = 2;
        vm.ReceivedQuantity = 3;
        Assert.AreEqual(5, vm.Difference);

        vm.AlreadyReceived = 10;
        vm.ReceivedQuantity = 1;
        Assert.AreEqual(0, vm.Difference);
    }
}
