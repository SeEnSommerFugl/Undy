using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels
{
    public class PurchaseOrderViewModel : BaseViewModel
    {
        private IBaseRepository<PurchaseOrder, Guid> _purchaseOrderRepo;
        private IBaseRepository<Stock, Guid> _productCatalogueRepo;

        public PurchaseOrderViewModel(IBaseRepository<PurchaseOrder, Guid> purchaseOrderRepo, IBaseRepository<Stock, Guid> productCatalogueRepo)
        {
            _purchaseOrderRepo = purchaseOrderRepo;
            _productCatalogueRepo = productCatalogueRepo;
        }
    }
}
