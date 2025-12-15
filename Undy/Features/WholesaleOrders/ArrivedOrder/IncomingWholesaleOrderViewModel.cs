namespace Undy.Features.ViewModel
{
    public class IncomingWholesaleOrderViewModel : BaseViewModel
    {
        private readonly IBaseRepository<WholesaleOrder, Guid> _wholesaleOrderRepo;
        private readonly IBaseRepository<Product, Guid> _productRepo;
        private readonly IBaseRepository<ProductWholesaleOrder, Guid> _productWholesaleOrderRepo;

        public ObservableCollection<WholesaleOrder> WholesaleOrders => _wholesaleOrderRepo.Items;
        public ObservableCollection<ProductWholesaleOrder> ProductWholesaleOrders => _productWholesaleOrderRepo.Items;
        public ICollectionView WholesaleView { get; }
        public ICollectionView LinesView { get; }
        public ObservableCollection<IncomingOrderLineViewModel> Lines { get; }

        public ICommand ConfirmOrderCommand { get; }

        private bool _isFullyReceived;
        private string _statusMessage;

        public IncomingWholesaleOrderViewModel(
            IBaseRepository<WholesaleOrder, Guid> wholesaleOrderRepo,
            IBaseRepository<Product, Guid> productRepo,
            IBaseRepository<ProductWholesaleOrder, Guid> productWholesaleOrderRepo)
        {
            _wholesaleOrderRepo = wholesaleOrderRepo;
            _productRepo = productRepo;
            _productWholesaleOrderRepo = productWholesaleOrderRepo;

            WholesaleView = CollectionViewSource.GetDefaultView(WholesaleOrders);
            WholesaleView.Filter = WholesaleFilter;
            Lines = new ObservableCollection<IncomingOrderLineViewModel>();
            LinesView = CollectionViewSource.GetDefaultView(Lines);

            ConfirmOrderCommand = new RelayCommand(
                async _ => await ConfirmAsync(),
                _ => SelectedOrder != null
            );
        }

        // ---------- Properties ----------

        private WholesaleOrder? _selectedOrder;
        public WholesaleOrder? SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                if (SetProperty(ref _selectedOrder, value))
                {
                    LoadLinesForSelectedOrder();
                    //(ConfirmOrderCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private bool WholesaleFilter(object obj)
        {
            if (obj is not WholesaleOrder order) return false;
            bool isNotReceived = order.OrderStatus != "Modtaget";
            return isNotReceived;
        }

        // ---------- Hent linjer når SelectedOrder ændres ----------
        private void LoadLinesForSelectedOrder()
        {
            if (SelectedOrder == null)
                return;
            Lines.Clear();

            var orderLines = _productWholesaleOrderRepo.Items;

            if (SelectedOrder != null)
            {
                foreach (var line in orderLines.Where(l => l.WholesaleOrderID == SelectedOrder.WholesaleOrderID))
                {
                    // slå produktet op for navn/nummer (hvis det findes)
                    var product = _productRepo.Items
                        .FirstOrDefault(p => p.ProductID == line.ProductID);
                    Lines.Add(new IncomingOrderLineViewModel
                    {
                        WholesaleOrderID = line.WholesaleOrderID,
                        ProductID = line.ProductID,
                        ProductNumber = product?.ProductNumber ?? string.Empty,
                        ProductName = product?.ProductName ?? string.Empty,
                        OrderedQuantity = line.Quantity,
                        AlreadyReceived = line.QuantityReceived,
                        ReceivedQuantity = 0
                    });
                }
            }
            LinesView.Refresh();
        }

        // ---------- Bekræft varemodtagelse ----------
        private async Task ConfirmAsync()
        {
            if (SelectedOrder == null)
            {
                StatusMessage = "Vælg en ordre først.";
                return;
            }


            //if (SelectedOrder == null)
            //{
            //    StatusMessage = "Vælg en ordre først.";
            //    return;
            //}

            //try
            //{
            //    IsBusy = true;

            //    // vi skal bruge de ekstra metoder på det KONKRETE repo
            //    if (_wholesaleOrderRepo is not WholesaleOrderDBRepository concreteRepo)
            //    {
            //        StatusMessage = "Kan ikke bekræfte varemodtagelse (forkert repo-type).";
            //        return;
            //    }

            //    if (IsFullyReceived)
            //    {
            //        // fuld modtagelse
            //        await concreteRepo.ConfirmFullReceiveAsync(SelectedOrder.WholesaleOrderNumber);
            //    }
            //    else
            //    {
            //        // delvis modtagelse 
            //        foreach (var line in Lines.Where(l => l.ReceivedQuantity > 0))
            //        {
            //            var maxRemaining = line.OrderedQuantity - line.AlreadyReceived;
            //            if (line.ReceivedQuantity > maxRemaining)
            //            {

            //                continue;
            //            }

            //            await concreteRepo.ConfirmPartialReceiveAsync(
            //                SelectedOrder.WholesaleOrderNumber,
            //                line.ProductNumber,
            //                line.ReceivedQuantity);
            //        }
            //    }

            //    // reload data efter opdatering
            //    WholesaleView.Refresh();

            //    StatusMessage = "Varemodtagelse registreret.";
            //}
            //catch
            //{
            //    StatusMessage = "Der opstod en fejl ved varemodtagelsen.";
            //}
            //finally
            //{
            //    IsBusy = false;
            //}
        }
    }
}