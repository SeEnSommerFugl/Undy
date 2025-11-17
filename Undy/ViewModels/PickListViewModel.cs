using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels
{
    public class PickListViewModel : BaseViewModel
    {
        private IBaseRepository<SalesOrder, Guid> _salesOrderRepo;
        private IBaseRepository<Product, Guid> _productRepo;

        public PickListViewModel(IBaseRepository<SalesOrder, Guid> salesOrderRepo, IBaseRepository<Product, Guid> productRepo)
        {
            _salesOrderRepo = salesOrderRepo;
            _productRepo = productRepo;
        }
    }
}
