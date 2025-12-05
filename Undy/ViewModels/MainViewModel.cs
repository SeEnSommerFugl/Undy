using Undy.ViewModels.Helpers;

namespace Undy.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public Navigation Nav { get; } = new();
        private readonly WholesaleOrderViewModel _wholesaleOrderViewModel;
        private readonly IncomingWholeSaleOrderViewModel _incomingWholeSaleOrderViewModel;
        private readonly PickListViewModel _pickListViewModel;
        private readonly SalesOrderViewModel _salesOrderViewModel;
        private readonly PaymentViewModel _paymentViewModel;
        private readonly TestReturnOrderViewModel _testReturnOrderViewModel;
        private readonly TestSalesOrderViewModel _testSalesOrderViewModel;
        private readonly TestWholesaleOrderViewModel _testWholesaleOrderViewModel;
        private readonly StartPageViewModel _startPageViewModel;


        public MainViewModel(StartPageViewModel startPageViewModel, WholesaleOrderViewModel wholesaleOrderViewModel,
            IncomingWholeSaleOrderViewModel incomingWholeSaleOrderViewModel, PickListViewModel pickListViewModel, 
            SalesOrderViewModel salesOrderViewModel, PaymentViewModel paymentViewModel, 
            TestReturnOrderViewModel testReturnOrderViewModel, TestSalesOrderViewModel testSalesOrderViewModel, 
            TestWholesaleOrderViewModel testWholesaleOrderViewModel)
        {
            _wholesaleOrderViewModel = wholesaleOrderViewModel;
            _incomingWholeSaleOrderViewModel = incomingWholeSaleOrderViewModel;
            _pickListViewModel = pickListViewModel;
            _salesOrderViewModel = salesOrderViewModel;
            _paymentViewModel = paymentViewModel;
            _testReturnOrderViewModel = testReturnOrderViewModel;
            _testSalesOrderViewModel = testSalesOrderViewModel;
            _testWholesaleOrderViewModel = testWholesaleOrderViewModel;
            _startPageViewModel = startPageViewModel;

            Nav.AddRange(
                ("StartPage", "Startside", _startPageViewModel),
                ("WholesaleOrder", "Indkøb", _wholesaleOrderViewModel),
                ("IncomingWholeSaleOrder", "Vare modtagelse", _incomingWholeSaleOrderViewModel),
                ("PickLists", "Pluklister", _pickListViewModel),
                ("SalesOrders", "Salgs ordrer", _salesOrderViewModel),
                ("Payments", "Betalinger", _paymentViewModel),
                ("TestReturnOrders", "Test returneringer", _testReturnOrderViewModel),
                ("TestWholesaleOrder", "Test indkøbsordrer", _testWholesaleOrderViewModel),
                ("TestSalesOrders", "Test salgsordrer", _testSalesOrderViewModel)
            );

            Nav.Initialize(defaultId: "StartPage");
        }
    }
}
