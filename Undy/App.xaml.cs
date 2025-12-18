namespace Undy
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // ----- Culture Info ----- //
            var culture = new CultureInfo("da-DK");

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(culture.IetfLanguageTag)));

            // ----- Shared Repository Instances ----- //
            IBaseRepository<WholesaleOrder, Guid> wholesaleOrderRepo = new WholesaleOrderDBRepository();
            IBaseRepository<SalesOrder, Guid> salesOrderRepo = new SalesOrderDBRepository();
            IBaseRepository<ReturnOrder, Guid> returnOrderRepo = new ReturnOrderDBRepository();

            IBaseRepository<WholesaleOrderDisplay, Guid> wholesaleOrderDisplayRepo = new WholesaleOrderDisplayDBRepository();

            IBaseRepository<Customer, Guid> customerRepo = new CustomerDBRepository();
            IBaseRepository<Product, Guid> productRepo = new ProductDBRepository();

            // Display projection repo (vw_CustomerSalesOrders)
            IBaseRepository<CustomerSalesOrderDisplay, Guid> customerSalesOrderDisplayRepo =
                new CustomerSalesOrderDisplayDBRepository();

            // Line repos (composite keys -> no IBaseRepository<Guid>)
            var salesOrderLineRepo = new SalesOrderLineDBRepository();
            var wholesaleOrderLineRepo = new WholesaleOrderLineDBRepository();

            // ----- ViewModels ----- //
            var startPageRepo = new StartPageDBRepository();
            var startPageVM = new StartPageViewModel(startPageRepo);


            var wholesaleOrderVM = new WholesaleOrderViewModel(
                wholesaleOrderRepo,
                wholesaleOrderDisplayRepo,
                productRepo,
                wholesaleOrderLineRepo);



            var incomingWholesaleOrderVM = new IncomingWholesaleOrderViewModel(
                wholesaleOrderRepo,
                productRepo,
                wholesaleOrderLineRepo);

            // IMPORTANT:
            // SalesOrderVM and TestSalesOrderVM must share the same salesOrderRepo instance.
            var salesOrderVM = new SalesOrderViewModel(
                salesOrderRepo,
                salesOrderLineRepo);

            var paymentVM = new PaymentViewModel(
                salesOrderRepo,
                customerSalesOrderDisplayRepo);

            var testReturnOrderVM = new TestReturnOrderViewModel(
                returnOrderRepo,
                salesOrderRepo,
                salesOrderLineRepo);

            var testSalesOrderVM = new TestSalesOrderViewModel(
                salesOrderRepo,
                productRepo,
                salesOrderLineRepo,
                customerRepo);

            var productVM = new ProductViewModel(productRepo);

            var mainVM = new MainViewModel(
                startPageVM,
                wholesaleOrderVM,
                incomingWholesaleOrderVM,
                salesOrderVM,
                paymentVM,
                testReturnOrderVM,
                testSalesOrderVM,
                productVM
            );

            // ----- Main Window ----- //
            var mainWindow = new MainWindow(mainVM);
            mainWindow.Show();

            // ----- Load data from DB ----- //
            try
            {
                await productRepo.InitializeAsync();
                await customerRepo.InitializeAsync();

                await wholesaleOrderRepo.InitializeAsync();
                await wholesaleOrderDisplayRepo.InitializeAsync();

                await salesOrderRepo.InitializeAsync();
                await customerSalesOrderDisplayRepo.InitializeAsync();

                await returnOrderRepo.InitializeAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "Startup error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Shutdown(-1);
            }
        }
    }
}
