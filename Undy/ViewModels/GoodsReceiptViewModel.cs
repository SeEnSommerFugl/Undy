using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels
{
    public class GoodsReceiptViewModel : BaseViewModel
    {
        private IBaseRepository<PurchaseOrder> _purchaseOrderRepo;
        private IBaseRepository<Product> _productRepo;

        public GoodsReceiptViewModel(IBaseRepository<PurchaseOrder> purchaseOrderRepo, IBaseRepository<Product> productRepo)
        {
            _purchaseOrderRepo = purchaseOrderRepo;
            _productRepo = productRepo;
        }
    }
}
