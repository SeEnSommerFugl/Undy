using Undy.ViewModels.Helpers;

namespace Undy.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public Navigation Nav { get; } = new();
        private readonly WholesaleOrderViewModel _wholesaleOrderViewModel;
        private readonly IncomingWholesaleOrderViewModel _incomingWholesaleOrderViewModel;
        private readonly SalesOrderViewModel _salesOrderViewModel;
        private readonly PaymentViewModel _paymentViewModel;
        private readonly TestReturnOrderViewModel _testReturnOrderViewModel;
        private readonly TestSalesOrderViewModel _testSalesOrderViewModel;
        private readonly TestWholesaleOrderViewModel _testWholesaleOrderViewModel;
        private readonly StartPageViewModel _startPageViewModel;


        public MainViewModel(StartPageViewModel startPageViewModel, WholesaleOrderViewModel wholesaleOrderViewModel,
            IncomingWholesaleOrderViewModel incomingWholesaleOrderViewModel, 
            SalesOrderViewModel salesOrderViewModel, PaymentViewModel paymentViewModel, 
            TestReturnOrderViewModel testReturnOrderViewModel, TestSalesOrderViewModel testSalesOrderViewModel, 
            TestWholesaleOrderViewModel testWholesaleOrderViewModel)
        {
            _wholesaleOrderViewModel = wholesaleOrderViewModel;
            _incomingWholesaleOrderViewModel = incomingWholesaleOrderViewModel;
            _salesOrderViewModel = salesOrderViewModel;
            _paymentViewModel = paymentViewModel;
            _testReturnOrderViewModel = testReturnOrderViewModel;
            _testSalesOrderViewModel = testSalesOrderViewModel;
            _testWholesaleOrderViewModel = testWholesaleOrderViewModel;
            _startPageViewModel = startPageViewModel;

            Nav.AddRange(
                ("StartPage", "Startside", _startPageViewModel),
                ("WholesaleOrder", "Indkøb", _wholesaleOrderViewModel),
                ("IncomingWholesaleOrder", "Vare modtagelse", _incomingWholesaleOrderViewModel),
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
