namespace Undy.Features.ViewModel
{
    public class WholesaleOrderViewModel : BaseViewModel
    {
        private readonly IBaseRepository<WholesaleOrderDisplay, Guid> _wholesaleOrderDisplayRepo;
        private readonly IBaseRepository<ProductWholesaleOrder, Guid> _productWholesaleOrderRepo;
        private readonly IBaseRepository<Product, Guid> _productRepo;
        private readonly ICollectionView _wholesaleView;

        public ObservableCollection<WholesaleOrderDisplay> WholesaleOrders => _wholesaleOrderDisplayRepo.Items;
        public ICollectionView WholesaleView => _wholesaleView;

        public WholesaleOrderViewModel(IBaseRepository<WholesaleOrderDisplay, Guid> wholesaleOrderDisplayRepo, IBaseRepository<Product, Guid> productRepo, IBaseRepository<ProductWholesaleOrder, Guid> productWholesaleOrderRepo)
        {
            _wholesaleOrderDisplayRepo = wholesaleOrderDisplayRepo;
            _productWholesaleOrderRepo = productWholesaleOrderRepo;
            _productRepo = productRepo;


            _wholesaleView = CollectionViewSource.GetDefaultView(WholesaleOrders);
            _wholesaleView.SortDescriptions.Add(new SortDescription(nameof(WholesaleOrder.WholesaleOrderDate), ListSortDirection.Descending));

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
