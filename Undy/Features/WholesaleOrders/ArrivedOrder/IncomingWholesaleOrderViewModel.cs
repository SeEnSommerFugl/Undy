namespace Undy.Features.ViewModel
{
    public class IncomingWholesaleOrderViewModel : BaseViewModel
    {
        private readonly IBaseRepository<WholesaleOrder, Guid> _wholesaleOrderRepo;
        private readonly IBaseRepository<Product, Guid> _productRepo;
        private readonly IBaseRepository<ProductWholesaleOrder, Guid> _productWholesaleOrderRepo;
        public ObservableCollection<WholesaleOrder> WholesaleOrders => _wholesaleOrderRepo.Items;
        public ICollectionView WholesaleView { get; }

        public ObservableCollection<IncomingOrderLineViewModel> Lines { get; }
        private WholesaleOrder? _selectedOrder;
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
            Lines = new ObservableCollection<IncomingOrderLineViewModel>();

            ConfirmOrderCommand = new RelayCommand(
                async _ => await ConfirmAsync(),
                _ => SelectedOrder != null
            );
        }

        // ---------- Properties ----------


        public WholesaleOrder? SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                if (SetProperty(ref _selectedOrder, value))
                {
                    _ = LoadLinesForSelectedOrderAsync();
                    (ConfirmOrderCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsFullyReceived
        {
            get => _isFullyReceived;
            set
            {
                if (SetProperty(ref _isFullyReceived, value))
                {
                    if (_isFullyReceived)
                    {
                        
                        foreach (var line in Lines)
                        {
                            var remaining = line.OrderedQuantity - line.AlreadyReceived;
                            if (remaining < 0)
                                remaining = 0;

                            line.ReceivedQuantity = remaining;
                        }
                    }
                    else
                    {
                       
                        foreach (var line in Lines)
                        {
                            line.ReceivedQuantity = 0;
                        }
                    }
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }




        // ---------- Hent linjer når SelectedOrder ændres ----------

        private async Task LoadLinesForSelectedOrderAsync()
        {
            Lines.Clear();
            IsFullyReceived = false;

            if (SelectedOrder == null)
                return;

            try
            {
                IsBusy = true;

                // find alle linjer for den valgte ordre
                var orderLines = _productWholesaleOrderRepo.Items
                    .Where(l => l.WholesaleOrderID == SelectedOrder.WholesaleOrderID);

                foreach (var line in orderLines)
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

                // er alle linjer allerede fuldt modtaget?
                var allAlreadyReceived = Lines.All(l => l.OrderedQuantity <= l.AlreadyReceived);
                IsFullyReceived = allAlreadyReceived;

                StatusMessage = string.Empty;
            }
            finally
            {
                IsBusy = false;
            }
        }

        // ---------- Bekræft varemodtagelse ----------

        private async Task ConfirmAsync()
        {
            if (SelectedOrder == null)
            {
                StatusMessage = "Vælg en ordre først.";
                return;
            }

            try
            {
                IsBusy = true;

                // vi skal bruge de ekstra metoder på det KONKRETE repo
                if (_wholesaleOrderRepo is not WholesaleOrderDBRepository concreteRepo)
                {
                    StatusMessage = "Kan ikke bekræfte varemodtagelse (forkert repo-type).";
                    return;
                }

                if (IsFullyReceived)
                {
                    // fuld modtagelse
                    await concreteRepo.ConfirmFullReceiveAsync(SelectedOrder.WholesaleOrderNumber);
                }
                else
                {
                    // delvis modtagelse 
                    foreach (var line in Lines.Where(l => l.ReceivedQuantity > 0))
                    {
                        var maxRemaining = line.OrderedQuantity - line.AlreadyReceived;
                        if (line.ReceivedQuantity > maxRemaining)
                        {
                            
                            continue;
                        }

                        await concreteRepo.ConfirmPartialReceiveAsync(
                            SelectedOrder.WholesaleOrderNumber,
                            line.ProductNumber,
                            line.ReceivedQuantity);
                    }
                }

                // reload data efter opdatering
                await _wholesaleOrderRepo.InitializeAsync();
                await _productWholesaleOrderRepo.InitializeAsync();
                await LoadLinesForSelectedOrderAsync();

                StatusMessage = "Varemodtagelse registreret.";
            }
            catch
            {
                StatusMessage = "Der opstod en fejl ved varemodtagelsen.";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}