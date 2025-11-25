using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels
{
    public class SalesOrderViewModel : BaseViewModel
    {
        private IBaseRepository<SalesOrder, Guid> _salesOrderRepo;
        private IBaseRepository<Stock, Guid> _productCatalogueRepo;
        private IBaseRepository<Product, Guid> _productRepo;

        public SalesOrderViewModel(IBaseRepository<SalesOrder, Guid> salesOrderRepo, IBaseRepository<Stock, Guid> productCatalogueRepo, IBaseRepository<Product, Guid> productRepo)
        {
            _salesOrderRepo = salesOrderRepo;
            _productCatalogueRepo = productCatalogueRepo;
            _productRepo = productRepo;

            //public ObservableCollection<SalesOrder> SalesOrders { get; set; }
        }
    }
}

