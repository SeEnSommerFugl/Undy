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
        private readonly StartPageViewModel _startPageViewModel;

        public MainViewModel(PurchaseOrderViewModel purchaseOrderViewModel, GoodsReceiptViewModel goodsReceiptViewModel, PickListViewModel pickListViewModel, SalesOrderViewModel salesOrderViewModel, PaymentViewModel paymentViewModel, StartPageViewModel startPageViewModel)
        {
            _purchaseOrderViewModel = purchaseOrderViewModel;
            _goodsReceiptViewModel = goodsReceiptViewModel;
            _pickListViewModel = pickListViewModel;
            _salesOrderViewModel = salesOrderViewModel;
            _paymentViewModel = paymentViewModel;
            _startPageViewModel = startPageViewModel;

            Nav.AddRange(
                ("StartPage", "Startside", _startPageViewModel),
                ("PurchaseOrders", "Indkøb", _purchaseOrderViewModel),
                ("GoodsReceipts", "Vare modtagelse", _goodsReceiptViewModel),
                ("PickLists", "Pluklister", _pickListViewModel),
                ("SalesOrders", "Salgs ordrer", _salesOrderViewModel),
                ("Payments", "Betalinger", _paymentViewModel)
            );

            Nav.Initialize(defaultId: "StartPage");
        }
    }
}
