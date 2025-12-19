namespace Undy.Features.ViewModel
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
        private readonly StartPageViewModel _startPageViewModel;
        private readonly ProductViewModel _productViewModel;


        public MainViewModel(StartPageViewModel startPageViewModel, WholesaleOrderViewModel wholesaleOrderViewModel,
            IncomingWholesaleOrderViewModel incomingWholesaleOrderViewModel,
            SalesOrderViewModel salesOrderViewModel, PaymentViewModel paymentViewModel,
            TestReturnOrderViewModel testReturnOrderViewModel, TestSalesOrderViewModel testSalesOrderViewModel,
            ProductViewModel productViewModel)

        {
            _wholesaleOrderViewModel = wholesaleOrderViewModel;
            _incomingWholesaleOrderViewModel = incomingWholesaleOrderViewModel;
            _salesOrderViewModel = salesOrderViewModel;
            _paymentViewModel = paymentViewModel;
            _testReturnOrderViewModel = testReturnOrderViewModel;
            _testSalesOrderViewModel = testSalesOrderViewModel;
            _startPageViewModel = startPageViewModel;
            _productViewModel = productViewModel;

            Nav.AddRange(
                ("StartPage", "Startside", _startPageViewModel),
                ("Products", "Produkter", _productViewModel),
                ("WholesaleOrder", "Opret Indkøb", _wholesaleOrderViewModel),
                ("IncomingWholesaleOrder", "Varemodtagelse", _incomingWholesaleOrderViewModel),
                ("SalesOrders", "Salgsordrer", _salesOrderViewModel),
                ("Payments", "Betalinger", _paymentViewModel),
                ("TestReturnOrders", "Test returneringer", _testReturnOrderViewModel),
                ("TestSalesOrders", "Test salgsordrer", _testSalesOrderViewModel)
            );

            Nav.Initialize(defaultId: "StartPage");
        }
    }
}
