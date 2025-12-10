using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using Undy.Data.Repository;
using Undy.Models;
using Undy.Views;
using Undy.Features.Base;
using Undy.Features.Helpers;
using Undy.Features.Links;
using Undy.Features.Main;
using Undy.Features.Payment;
using Undy.Features.Products;
using Undy.Features.ReturnOrders;
using Undy.Features.ReturnOrders.Demo;
using Undy.Features.SalesOrders;
using Undy.Features.SalesOrders.Demo;
using Undy.Features.StartPage;
using Undy.Features.WholesaleOrders;
using Undy.Features.WholesaleOrders.ArrivedOrder;
using Undy.Features.WholesaleOrders.Demo;


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

            // ----- Shared Instances ----- //
            IBaseRepository<Product, Guid> productRepo = new ProductDBRepository();
            IBaseRepository<WholesaleOrder, Guid> wholesaleOrderRepo = new WholesaleOrderDBRepository();
            IBaseRepository<SalesOrder, Guid> salesOrderRepo = new SalesOrderDBRepository();
            IBaseRepository<ReturnOrder, Guid> testReturnRepo = new ReturnOrderDBRepository();
            IBaseRepository<SalesOrder, Guid> testSalesOrderRepo = new SalesOrderDBRepository();
            IBaseRepository<SalesOrderDisplay, Guid> salesOrderDisplayRepo = new SalesOrderDisplayDBRepository();
            IBaseRepository<ProductWholesaleOrder, Guid> productWholesaleOrderRepo = new ProductWholesaleOrderDBRepository();
            IBaseRepository<ProductSalesOrder, Guid> productSalesOrderRepo = new ProductSalesOrderDBRepository();
            IBaseRepository<Customer, Guid> customerRepo = new CustomerDBRepository();
            IBaseRepository<CustomerSalesOrderDisplay, Guid> customerSalesOrderDisplayRepo = new CustomerSalesOrderDisplayDBRepository();

            //----- ViewModels ----- //
            var startPageVM = new StartPageViewModel();
            var wholesaleOrderVM = new WholesaleOrderViewModel(wholesaleOrderRepo, productRepo);
            var incomingWholesaleOrderVM = new IncomingWholesaleOrderViewModel(wholesaleOrderRepo, productRepo, productWholesaleOrderRepo);
            var salesOrderVM = new SalesOrderViewModel(salesOrderDisplayRepo, productSalesOrderRepo);
            var paymentVM = new PaymentViewModel(salesOrderRepo, customerSalesOrderDisplayRepo);
            var testReturnOrderVM = new TestReturnOrderViewModel(testReturnRepo);
            var testSalesOrderVM = new TestSalesOrderViewModel(salesOrderRepo, productRepo, productSalesOrderRepo, customerRepo);
            var testWholesaleOrderVM = new TestWholesaleOrderViewModel(wholesaleOrderRepo, productRepo);

            var mainVM = new MainViewModel(
                startPageVM,
                wholesaleOrderVM,
                incomingWholesaleOrderVM,
                salesOrderVM,
                paymentVM,
                testReturnOrderVM,
                testSalesOrderVM,
                testWholesaleOrderVM

            );

            // ----- Main Window ----- //
            var mainWindow = new MainWindow(mainVM);
            mainWindow.Show();

            await Task.WhenAll(
            //startPageRepo.InitializeAsync(),
            productRepo.InitializeAsync(),
            //stockRepo.InitializeAsync(),
            wholesaleOrderRepo.InitializeAsync(),
            salesOrderRepo.InitializeAsync(),
            salesOrderDisplayRepo.InitializeAsync(),
            customerRepo.InitializeAsync(),
            //productSalesOrderRepo.InitializeAsync(),
            //testReturnRepo.InitializeAsync(),
            //testPurchaseOrderRepo.InitializeAsync(),
            //testSalesOrderRepo.InitializeAsync(),
            Task.CompletedTask
            );

        }
    }
}
