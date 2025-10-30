using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels
{
    public class PickListViewModel : BaseViewModel
    {
        private IBaseRepository<SalesOrder> _salesOrderRepo;
        private IBaseRepository<Product> _productRepo;

        public PickListViewModel(IBaseRepository<SalesOrder> salesOrderRepo, IBaseRepository<Product> productRepo)
        {
            _salesOrderRepo = salesOrderRepo;
            _productRepo = productRepo;
        }
    }
}
