using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels
{
    public class SalesOrderViewModel : BaseViewModel
    {
        private IBaseRepository<SalesOrder> _salesOrderRepo;
        private IBaseRepository<ProductCatalogue> _productCatalogueRepo;
        private IBaseRepository<Product> _productRepo;

        public SalesOrderViewModel(IBaseRepository<SalesOrder> salesOrderRepo, IBaseRepository<ProductCatalogue> productCatalogueRepo, IBaseRepository<Product> productRepo)
        {
            _salesOrderRepo = salesOrderRepo;
            _productCatalogueRepo = productCatalogueRepo;
            _productRepo = productRepo;
        }
    }
}
