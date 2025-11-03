using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels
{
    public class PurchaseOrderViewModel : BaseViewModel
    {
        private IBaseRepository<PurchaseOrder> _purchaseOrderRepo;
        private IBaseRepository<Stock> _productCatalogueRepo;

        public PurchaseOrderViewModel(IBaseRepository<PurchaseOrder> purchaseOrderRepo, IBaseRepository<Stock> productCatalogueRepo)
        {
            _purchaseOrderRepo = purchaseOrderRepo;
            _productCatalogueRepo = productCatalogueRepo;
        }
    }
}
