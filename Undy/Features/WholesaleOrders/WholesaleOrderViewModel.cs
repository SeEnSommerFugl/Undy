using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Undy.Data.Repository;
using Undy.Models;

namespace Undy.Features.ViewModel
{
    public class WholesaleOrderViewModel : BaseViewModel
    {
        private readonly IBaseRepository<WholesaleOrderDisplay, Guid> _wholesaleOrderDisplayRepo;
        private readonly IBaseRepository<Product, Guid> _productRepo;
        private readonly ICollectionView _wholesaleView;

        public ObservableCollection<WholesaleOrderDisplay> WholesaleOrders => _wholesaleOrderDisplayRepo.Items;
        public ICollectionView WholesaleView => _wholesaleView;

        private int? _productNumberSearch;
        public int? ProductNumberSearch
        {
            get => _productNumberSearch;
            set
            {
                if (SetProperty(ref _productNumberSearch, value))
                    _wholesaleView.Refresh();
            }
        }

        private string? _supplierSearch;
        public string? SupplierSearch
        {
            get => _supplierSearch;
            set
            {
                if (SetProperty(ref _supplierSearch, value))
                    _wholesaleView.Refresh();
            }
        }

        private string? _productNameSearch;
        public string? ProductNameSearch
        {
            get => _productNameSearch;
            set
            {
                if (SetProperty(ref _productNameSearch, value))
                    _wholesaleView.Refresh();
            }
        }

        public WholesaleOrderViewModel(
            IBaseRepository<WholesaleOrderDisplay, Guid> wholesaleOrderDisplayRepo,
            IBaseRepository<Product, Guid> productRepo)
        {
            _wholesaleOrderDisplayRepo = wholesaleOrderDisplayRepo;
            _productRepo = productRepo;

            _wholesaleView = CollectionViewSource.GetDefaultView(WholesaleOrders);
            _wholesaleView.SortDescriptions.Add(
                new SortDescription(nameof(WholesaleOrderDisplay.WholesaleOrderDate), ListSortDirection.Descending));
            _wholesaleView.Filter = FilterWholesaleOrders;
        }

        private bool FilterWholesaleOrders(object obj)
        {
            if (obj is not WholesaleOrderDisplay wo)
                return false;

            if (ProductNumberSearch.HasValue)
            {
                if (!int.TryParse(wo.ProductNumber, out var pn) || pn != ProductNumberSearch.Value)
                    return false;
            }

            if (!string.IsNullOrWhiteSpace(ProductNameSearch))
            {
                if (wo.ProductName is null ||
                    !wo.ProductName.Contains(ProductNameSearch, System.StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            // SupplierSearch has no matching column in WholesaleOrderDisplay right now, so it is intentionally ignored.
            return true;
        }
    }
}
