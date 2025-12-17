namespace Undy.Features.ViewModel
{
    public class IncomingWholesaleOrderViewModel : BaseViewModel
    {
        private readonly IBaseRepository<WholesaleOrder, Guid> _wholesaleOrderRepo;
        private readonly IBaseRepository<Product, Guid> _productRepo;
        private readonly WholesaleOrderLineDBRepository _wholesaleOrderLineRepo;
        public ObservableCollection<WholesaleOrder> WholesaleOrders => _wholesaleOrderRepo.Items;

        public ICollectionView WholesaleView { get; }
        public ICollectionView LinesView { get; }



        public IncomingWholesaleOrderViewModel(
            IBaseRepository<WholesaleOrder, Guid> wholesaleOrderRepo,
            IBaseRepository<Product, Guid> productRepo,
            WholesaleOrderLineDBRepository wholesaleOrderLineRepo)
        {
            _wholesaleOrderRepo = wholesaleOrderRepo;
            _productRepo = productRepo;
            _wholesaleOrderLineRepo = wholesaleOrderLineRepo;

            WholesaleView = CollectionViewSource.GetDefaultView(WholesaleOrders);


            SelectedOrderLines = new ObservableCollection<WholesaleOrderLine>();
            LinesView = CollectionViewSource.GetDefaultView(SelectedOrderLines);
        }

        private WholesaleOrder? _selectedWholesaleOrder;
        public WholesaleOrder? SelectedWholesaleOrder
        {
            get => _selectedWholesaleOrder;
            set
            {
                if (SetProperty(ref _selectedWholesaleOrder, value))
                    _ = LoadSelectedOrderLinesAsync();
            }
        }

        public ObservableCollection<WholesaleOrderLine> SelectedOrderLines { get; }

        private async Task LoadSelectedOrderLinesAsync()
        {
            SelectedOrderLines.Clear();

            if (SelectedWholesaleOrder is null)
                return;

            var lines = await _wholesaleOrderLineRepo.GetByWholesaleOrderIdAsync(SelectedWholesaleOrder.WholesaleOrderID);
            foreach (var line in lines)
            {
                SelectedOrderLines.Add(line);
            }
        }
    }
}
