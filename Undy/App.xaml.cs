using System.Windows;
using Undy.Data.Repository;
using Undy.Models;
using Undy.ViewModels;
using Undy.Views;

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

            // ----- Shared Instances ----- //
            IBaseRepository<Product, Guid> productRepo = new ProductDBRepository();
            IBaseRepository<WholesaleOrder, Guid> wholesaleOrderRepo = new WholesaleOrderDBRepository();
            IBaseRepository<SalesOrder, Guid> salesOrderRepo = new SalesOrderDBRepository();
            IBaseRepository<ReturnOrder, Guid> testReturnRepo = new ReturnOrderDBRepository();
            IBaseRepository<SalesOrder, Guid> testSalesOrderRepo = new SalesOrderDBRepository();
            IBaseRepository<SalesOrderDisplay, Guid> salesOrderDisplayRepo = new SalesOrderDisplayDBRepository();
            IBaseRepository<ProductWholesaleOrder, Guid> productWholesaleOrderRepo = new ProductWholesaleOrderDBRepository();

            //----- ViewModels ----- //
            var startPageVM = new StartPageViewModel();
            var wholesaleOrderVM = new WholesaleOrderViewModel(wholesaleOrderRepo, productRepo);
            var incomingWholesaleOrderVM = new IncomingWholesaleOrderViewModel(wholesaleOrderRepo, productRepo, productWholesaleOrderRepo);
            var pickListVM = new PickListViewModel(salesOrderRepo, productRepo);
            var salesOrderVM = new SalesOrderViewModel(salesOrderDisplayRepo);
            var paymentVM = new PaymentViewModel(salesOrderRepo);
            var testReturnOrderVM = new TestReturnOrderViewModel(testReturnRepo);
            var testSalesOrderVM = new TestSalesOrderViewModel(salesOrderRepo, productRepo);
            var testWholesaleOrderVM = new TestWholesaleOrderViewModel(wholesaleOrderRepo, productRepo);

            var mainVM = new MainViewModel(
                startPageVM,
                wholesaleOrderVM,
                incomingWholesaleOrderVM,
                pickListVM,
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
            //testReturnRepo.InitializeAsync(),
            //testPurchaseOrderRepo.InitializeAsync(),
            //testSalesOrderRepo.InitializeAsync(),
            Task.CompletedTask
            );

        }
    }
}
