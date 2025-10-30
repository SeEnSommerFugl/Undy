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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // ----- Shared Instances ----- //
            IBaseRepository<Product> productRepo = new ProductDBRepository();
            IBaseRepository<ProductCatalogue> productCatalogueRepo = new ProductCatalogueDBRepository();
            IBaseRepository<PurchaseOrder> purchaseOrderRepo = new PurchaseOrderDBRepository();
            IBaseRepository<SalesOrder> salesOrderRepo = new SalesOrderDBRepository();

            //----- ViewModels ----- //
            var purchaseOrderVM = new PurchaseOrderViewModel(purchaseOrderRepo, productCatalogueRepo);
            var goodsReceiptVM = new GoodsReceiptViewModel(purchaseOrderRepo, productRepo);
            var pickListVM = new PickListViewModel(salesOrderRepo, productRepo);
            var salesOrderVM = new SalesOrderViewModel(salesOrderRepo, productCatalogueRepo, productRepo);
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

        }
    }
}
