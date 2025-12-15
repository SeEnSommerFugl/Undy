namespace Undy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // ----- Culture Info ----- //
            var culture = new CultureInfo("da-DK");

            // For the current thread
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            // For any threads created later
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            // Tell WPF bindings to use the same culture (important for decimal comma etc.)
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(culture.IetfLanguageTag)));

            // ----- Shared Repository Instances ----- //
            IBaseRepository<WholesaleOrder, Guid> wholesaleOrderRepo = new WholesaleOrderDBRepository();
            IBaseRepository<SalesOrder, Guid> salesOrderRepo = new SalesOrderDBRepository();
            IBaseRepository<ReturnOrder, Guid> returnOrderRepo = new ReturnOrderDBRepository();

            IBaseRepository<WholesaleOrderDisplay, Guid> wholesaleOrderDisplayRepo = new WholesaleOrderDisplayDBRepository();
            IBaseRepository<ProductWholesaleOrder, Guid> productWholesaleOrderRepo = new ProductWholesaleOrderDBRepository();
            IBaseRepository<ProductSalesOrder, Guid> productSalesOrderRepo = new ProductSalesOrderDBRepository();

            IBaseRepository<Customer, Guid> customerRepo = new CustomerDBRepository();
            IBaseRepository<CustomerSalesOrder, Guid> customerSalesOrderRepo = new CustomerSalesOrderDBRepository();
            IBaseRepository<Product, Guid> productRepo = new ProductDBRepository();

            // ----- ViewModels ----- //
            var startPageVM = new StartPageViewModel();

            var wholesaleOrderVM = new WholesaleOrderViewModel(
                wholesaleOrderDisplayRepo,
                productRepo,
                productWholesaleOrderRepo);


            var incomingWholesaleOrderVM = new IncomingWholesaleOrderViewModel(
                wholesaleOrderRepo,
                productRepo,
                productWholesaleOrderRepo);

            // IMPORTANT:
            // SalesOrderVM og TestSalesOrderVM skal bruge SAMME salesOrderRepo instance,
            // ellers virker "live" opdatering ikke.
            var salesOrderVM = new SalesOrderViewModel(
                salesOrderRepo,
                productSalesOrderRepo);

            var paymentVM = new PaymentViewModel(
                salesOrderRepo,
                customerSalesOrderRepo);

            var testReturnOrderVM = new TestReturnOrderViewModel(
                returnOrderRepo,
                salesOrderRepo);

            var testSalesOrderVM = new TestSalesOrderViewModel(
                salesOrderRepo,
                productRepo,
                productSalesOrderRepo,
                customerRepo);

            var testWholesaleOrderVM = new TestWholesaleOrderViewModel(
                wholesaleOrderRepo,
                productRepo,
                productWholesaleOrderRepo);

            var productVM = new ProductViewModel(productRepo);

            var mainVM = new MainViewModel(
                startPageVM,
                wholesaleOrderVM,
                incomingWholesaleOrderVM,
                salesOrderVM,
                paymentVM,
                testReturnOrderVM,
                testSalesOrderVM,
                testWholesaleOrderVM,
                productVM
            );

            // ----- Main Window ----- //
            var mainWindow = new MainWindow(mainVM);
            mainWindow.Show();

            // ----- Load data from DB ----- //
            await productRepo.InitializeAsync();
            await customerRepo.InitializeAsync();

            await wholesaleOrderRepo.InitializeAsync();
            await productWholesaleOrderRepo.InitializeAsync();
            await wholesaleOrderDisplayRepo.InitializeAsync();

            await salesOrderRepo.InitializeAsync();
            await productSalesOrderRepo.InitializeAsync();
            await customerSalesOrderRepo.InitializeAsync();

            await returnOrderRepo.InitializeAsync();
        }
    }
}
