using Undy.ViewModels.Helpers;

namespace Undy.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public Navigation Nav { get; } = new();
        private readonly PurchaseOrderViewModel _purchaseOrderViewModel;
        private readonly GoodsReceiptViewModel _goodsReceiptViewModel;
        private readonly PickListViewModel _pickListViewModel;
        private readonly SalesOrderViewModel _salesOrderViewModel;
        private readonly PaymentViewModel _paymentViewModel;
        private readonly TestReturnOrderViewModel _testReturnOrderViewModel;
        private readonly TestSalesOrderViewModel _testSalesOrderViewModel;
        private readonly TestPurchaseOrderViewModel _testPurchaseOrderViewModel;

        public MainViewModel(PurchaseOrderViewModel purchaseOrderViewModel, GoodsReceiptViewModel goodsReceiptViewModel, PickListViewModel pickListViewModel, SalesOrderViewModel salesOrderViewModel, PaymentViewModel paymentViewModel, TestReturnOrderViewModel testReturnOrderViewModel, TestSalesOrderViewModel testSalesOrderViewModel, TestPurchaseOrderViewModel testPurchaseOrderView)
        {
            _purchaseOrderViewModel = purchaseOrderViewModel;
            _goodsReceiptViewModel = goodsReceiptViewModel;
            _pickListViewModel = pickListViewModel;
            _salesOrderViewModel = salesOrderViewModel;
            _paymentViewModel = paymentViewModel;
            _testReturnOrderViewModel = testReturnOrderViewModel;
            _testSalesOrderViewModel = testSalesOrderViewModel;
            _testPurchaseOrderViewModel = testPurchaseOrderViewModel;

            Nav.AddRange(
                ("PurchaseOrders", "Indkøb", _purchaseOrderViewModel),
                ("GoodsReceipts", "Vare modtagelse", _goodsReceiptViewModel),
                ("PickLists", "Pluklister", _pickListViewModel),
                ("SalesOrders", "Salgs ordrer", _salesOrderViewModel),
                ("Payments", "Betalinger", _paymentViewModel),
                ("TestReturnOrders", "Test returneringer", _testReturnOrderViewModel),
                ("TestPurchaseOrders", "Test indkøbsordrer", _testPurchaseOrderViewModel),
                ("TestSalesOrders", "Test salgsordrer", _testSalesOrderViewModel)
            );

            Nav.Initialize(defaultId: "PurchaseOrders");
        }
    }
}
