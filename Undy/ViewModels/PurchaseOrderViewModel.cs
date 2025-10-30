using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels
{
    public class PurchaseOrderViewModel : BaseViewModel
    {
        private IBaseRepository<PurchaseOrder> _purchaseOrderRepo;
        private IBaseRepository<ProductCatalogue> _productCatalogueRepo;

        public PurchaseOrderViewModel(IBaseRepository<PurchaseOrder> purchaseOrderRepo, IBaseRepository<ProductCatalogue> productCatalogueRepo)
        {
            _purchaseOrderRepo = purchaseOrderRepo;
            _productCatalogueRepo = productCatalogueRepo;
        }
    }
}
