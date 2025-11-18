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
            IBaseRepository<Stock, Guid> stockRepo = new StockDBRepository();
            IBaseRepository<PurchaseOrder, Guid> purchaseOrderRepo = new PurchaseOrderDBRepository();
            IBaseRepository<SalesOrder, Guid> salesOrderRepo = new SalesOrderDBRepository();

            //----- ViewModels ----- //
            var purchaseOrderVM = new PurchaseOrderViewModel(purchaseOrderRepo, stockRepo);
            var goodsReceiptVM = new GoodsReceiptViewModel(purchaseOrderRepo, productRepo);
            var pickListVM = new PickListViewModel(salesOrderRepo, productRepo);
            var salesOrderVM = new SalesOrderViewModel(salesOrderRepo, stockRepo, productRepo);
            var paymentVM = new PaymentViewModel(salesOrderRepo);

            var mainVM = new MainViewModel(
                purchaseOrderVM,
                goodsReceiptVM,
                pickListVM,
                salesOrderVM,
                paymentVM
            );

            // ----- Main Window ----- //
            var mainWindow = new MainWindow(mainVM);
            mainWindow.Show();

            await Task.WhenAll(
            //productRepo.InitializeAsync(),
            //stockRepo.InitializeAsync(),
            //purchaseOrderRepo.InitializeAsync(),
            //salesOrderRepo.InitializeAsync()
            Task.CompletedTask
            );

        }
    }
}
