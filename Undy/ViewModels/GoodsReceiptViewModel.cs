using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels
{
    public class GoodsReceiptViewModel : BaseViewModel
    {
        private IBaseRepository<PurchaseOrder, Guid> _purchaseOrderRepo;
        private IBaseRepository<Product, Guid> _productRepo;

        public GoodsReceiptViewModel(IBaseRepository<PurchaseOrder, Guid> purchaseOrderRepo, IBaseRepository<Product, Guid> productRepo)
        {
            _purchaseOrderRepo = purchaseOrderRepo;
            _productRepo = productRepo;
        }
    }
}
