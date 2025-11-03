using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels
{
    public class SalesOrderViewModel : BaseViewModel
    {
        private IBaseRepository<SalesOrder> _salesOrderRepo;
        private IBaseRepository<Stock> _productCatalogueRepo;
        private IBaseRepository<Product> _productRepo;

        public SalesOrderViewModel(IBaseRepository<SalesOrder> salesOrderRepo, IBaseRepository<Stock> productCatalogueRepo, IBaseRepository<Product> productRepo)
        {
            _salesOrderRepo = salesOrderRepo;
            _productCatalogueRepo = productCatalogueRepo;
            _productRepo = productRepo;
        }
    }
}
