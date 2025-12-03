using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Undy.Data.Repository;
using Undy.Models;
using Undy.ViewModels.Helpers;

namespace Undy.ViewModels
{
    public class WholesaleOrderViewModel : BaseViewModel
    {

        private IBaseRepository<WholesaleOrder, Guid> _wholesaleOrderRepo;
        private IBaseRepository<Stock, Guid> _productCatalogueRepo;
        
        private readonly ICollectionView _wholesaleView;
        public ObservableCollection<WholesaleOrder> WholesaleOrders => _wholesaleOrderRepo.Items;
        //public ObservableCollection<Stock> ProductCatalogue => _productCatalogueRepo.Items;

        public ICollectionView WholesaleView => _wholesaleView;

        public WholesaleOrderViewModel(IBaseRepository<WholesaleOrder, Guid> wholesaleOrderRepo, IBaseRepository<Stock, Guid> productCatalogueRepo)
        {
            _wholesaleOrderRepo = wholesaleOrderRepo;
            _productCatalogueRepo = productCatalogueRepo;


            _wholesaleView = CollectionViewSource.GetDefaultView(WholesaleOrders);
            _wholesaleView.SortDescriptions.Add(new SortDescription(nameof(WholesaleOrder.OrderDate), ListSortDirection.Descending));

        }

        private int? _productNumberSearch;
        public int? ProductNumberSearch
        {
            get => _productNumberSearch;
            set
            {
                if (SetProperty(ref _productNumberSearch, value))
                    _wholesaleView?.Refresh();
            }
        }

        private string? _supplierSearch;
        public string? SupplierSearch
        {
            get => _supplierSearch;
            set
            {
                if (SetProperty(ref _supplierSearch, value))
                    _wholesaleView?.Refresh();
            }
        }

        private string? _productNameSearch;
        public string? ProductNameSearch
        {
            get => _productNameSearch;
            set
            {
                if (SetProperty(ref _productNameSearch, value))
                    _wholesaleView?.Refresh();
            }
        }

    }
}
