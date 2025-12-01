using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels
{
    public class WholesaleOrderViewModel : BaseViewModel
    {
        private IBaseRepository<WholesaleOrder, Guid> _wholesaleOrderRepo;
        private IBaseRepository<Stock, Guid> _productCatalogueRepo;

        public WholesaleOrderViewModel(IBaseRepository<WholesaleOrder, Guid> wholesaleOrderRepo, IBaseRepository<Stock, Guid> productCatalogueRepo)
        {
            _wholesaleOrderRepo = wholesaleOrderRepo;
            _productCatalogueRepo = productCatalogueRepo;
        }
    }
}
