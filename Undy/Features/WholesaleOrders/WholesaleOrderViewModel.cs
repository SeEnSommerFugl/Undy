namespace Undy.Features.ViewModel
{
    public class WholesaleOrderViewModel : BaseViewModel
    {
        private readonly IBaseRepository<WholesaleOrder, Guid> _wholesaleOrderRepo;
        private readonly IBaseRepository<Product, Guid> _productRepo;
        private readonly ICollectionView _wholesaleView;

        public ObservableCollection<WholesaleOrder> WholesaleOrders => _wholesaleOrderRepo.Items;
        public ICollectionView WholesaleView => _wholesaleView;

        public WholesaleOrderViewModel(IBaseRepository<WholesaleOrder, Guid> wholesaleOrderRepo, IBaseRepository<Product, Guid> productRepo)
        {
            _wholesaleOrderRepo = wholesaleOrderRepo;
            _productRepo = productRepo;


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
