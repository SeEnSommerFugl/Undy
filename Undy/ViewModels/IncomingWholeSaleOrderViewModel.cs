using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels
{
    public class IncomingWholeSaleOrderViewModel : BaseViewModel
    {
        private IBaseRepository<WholesaleOrder, Guid> _wholesaleOrderRepo;
        private IBaseRepository<Product, Guid> _productRepo;

        public IncomingWholeSaleOrderViewModel(IBaseRepository<WholesaleOrder, Guid> purchaseOrderRepo, IBaseRepository<Product, Guid> productRepo)
        {
            _wholesaleOrderRepo = purchaseOrderRepo;
            _productRepo = productRepo;
        }
    }
}
