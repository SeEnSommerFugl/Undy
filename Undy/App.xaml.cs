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

            //----- ViewModels ----- //
            var wholesaleOrderVM = new WholesaleOrderViewModel(wholesaleOrderRepo, productRepo);
            var goodsReceiptVM = new GoodsReceiptViewModel(wholesaleOrderRepo, productRepo);
            var pickListVM = new PickListViewModel(salesOrderRepo, productRepo);
            var salesOrderVM = new SalesOrderViewModel(salesOrderDisplayRepo);
            var paymentVM = new PaymentViewModel(salesOrderRepo);
            var testReturnOrderVM = new TestReturnOrderViewModel(testReturnRepo);
            var testWholesaleOrderVM = new TestWholesaleOrderViewModel(wholesaleOrderRepo, productRepo);
            var testSalesOrderVM = new TestSalesOrderViewModel(salesOrderRepo, productRepo);
            var startPageVM = new StartPageViewModel();

            var mainVM = new MainViewModel(
                startPageVM,
                wholesaleOrderVM,
                goodsReceiptVM,
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
            //salesOrderRepo.InitializeAsync(),
            //testReturnRepo.InitializeAsync(),
            //testPurchaseOrderRepo.InitializeAsync(),
            //testSalesOrderRepo.InitializeAsync(),
            Task.CompletedTask
            );

        }
    }
}
