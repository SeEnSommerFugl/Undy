namespace Undy.Features.ViewModel
{
    public class WholesaleOrderViewModel : BaseViewModel
    {
        private readonly IBaseRepository<WholesaleOrder, Guid> _wholesaleOrderRepo;
        private readonly IBaseRepository<WholesaleOrderDisplay, Guid> _wholesaleOrderDisplayRepo;
        private readonly IBaseRepository<Product, Guid> _productRepo;
        private readonly WholesaleOrderLineDBRepository _wholesaleOrderLineRepo;

        private readonly ICollectionView _wholesaleView;

        // Bottom list (existing display)
        public ObservableCollection<WholesaleOrderDisplay> WholesaleOrders => _wholesaleOrderDisplayRepo.Items;
        public ICollectionView WholesaleView => _wholesaleView;

        // Products for top combo
        public ObservableCollection<Product> Products => _productRepo.Items;

        // Top create flow
        public ObservableCollection<WholesaleOrderLineEntryViewModel> WholesaleOrderLines { get; } = new();

        private Product? _selectedProduct;
        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        private DateTime? _expectedDeliveryDate;
        public DateTime? ExpectedDeliveryDate
        {
            get => _expectedDeliveryDate;
            set => SetProperty(ref _expectedDeliveryDate, value);
        }

        public ICommand ConfirmCommand { get; }
        public ICommand AddProductCommand { get; }
        public ICommand RemoveWholesaleOrderLineCommand { get; }

        private string? _confirmFeedback;
        public string? ConfirmFeedback
        {
            get => _confirmFeedback;
            set => SetProperty(ref _confirmFeedback, value);
        }

        private bool _isConfirmError;
        public bool IsConfirmError
        {
            get => _isConfirmError;
            set => SetProperty(ref _isConfirmError, value);
        }

        private void SetFeedbackError(string reason)
        {
            IsConfirmError = true;
            ConfirmFeedback = $"Fejl: {reason}";
        }

        private void SetFeedbackSuccess(int? orderNumber)
        {
            IsConfirmError = false;
            ConfirmFeedback = orderNumber.HasValue && orderNumber.Value > 0
                ? $"Indkøbsordre # {orderNumber.Value} oprettet"
                : "Indkøbsordre oprettet";
        }


        // Filters (existing)
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
            IBaseRepository<WholesaleOrder, Guid> wholesaleOrderRepo,
            IBaseRepository<WholesaleOrderDisplay, Guid> wholesaleOrderDisplayRepo,
            IBaseRepository<Product, Guid> productRepo,
            WholesaleOrderLineDBRepository wholesaleOrderLineRepo)
        {
            _wholesaleOrderRepo = wholesaleOrderRepo;
            _wholesaleOrderDisplayRepo = wholesaleOrderDisplayRepo;
            _productRepo = productRepo;
            _wholesaleOrderLineRepo = wholesaleOrderLineRepo;

            _wholesaleView = CollectionViewSource.GetDefaultView(WholesaleOrders);
            _wholesaleView.SortDescriptions.Add(
                new SortDescription(nameof(WholesaleOrderDisplay.WholesaleOrderDate), ListSortDirection.Descending));
            _wholesaleView.Filter = FilterWholesaleOrders;

            ConfirmCommand = new RelayCommand(async _ => await ConfirmAsync());
            AddProductCommand = new RelayCommand(_ => AddProduct(), _ => CanAddProduct());
            RemoveWholesaleOrderLineCommand = new RelayCommand(line =>
                RemoveWholesaleOrderLine(line as WholesaleOrderLineEntryViewModel));

            WholesaleOrderLines.CollectionChanged += (_, __) =>
            {
                // (Valgfrit) behold hvis du stadig bruger CanExecute et sted
                if (ConfirmCommand is RelayCommand rc)
                    rc.RaiseCanExecuteChanged();
            };



            ExpectedDeliveryDate ??= DateTime.Today;
        }

        public Task ConfirmAsync() => CreateWholesaleOrderAsync();

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

        private bool CanAddProduct() => SelectedProduct != null && Quantity > 0;


        private void AddProduct()
        {
            ConfirmFeedback = null;
            IsConfirmError = false;

            if (SelectedProduct == null)
                return;

            var existing = WholesaleOrderLines.FirstOrDefault(x => x.ProductID == SelectedProduct.ProductID);
            if (existing != null)
            {
                existing.Quantity += Quantity;

                if (ConfirmCommand is RelayCommand rc0)
                    rc0.RaiseCanExecuteChanged();

                return;
            }

            WholesaleOrderLines.Add(new WholesaleOrderLineEntryViewModel
            {
                ProductID = SelectedProduct.ProductID,
                ProductName = SelectedProduct.ProductName,
                UnitPrice = SelectedProduct.Price,
                Quantity = Quantity,
                QuantityReceived = 0
            });

            Quantity = 0;

            if (ConfirmCommand is RelayCommand rc1)
                rc1.RaiseCanExecuteChanged();
        }

        private void RemoveWholesaleOrderLine(WholesaleOrderLineEntryViewModel? line)
        {

            ConfirmFeedback = null;
            IsConfirmError = false;

            if (line == null) return;

            WholesaleOrderLines.Remove(line);

            if (ConfirmCommand is RelayCommand rc)
                rc.RaiseCanExecuteChanged();
        }

        private async Task CreateWholesaleOrderAsync()
        {
            // Clear previous feedback (we'll set a new one)
            ConfirmFeedback = null;
            IsConfirmError = false;

            if (WholesaleOrderLines.Count == 0)
            {
                IsConfirmError = true;
                SetFeedbackError("Du skal tilføje mindst ét produkt før du kan registrere ordren.");
                return;
            }

            try
            {
                var newOrder = new WholesaleOrder
                {
                    WholesaleOrderID = Guid.NewGuid(),
                    WholesaleOrderDate = DateOnly.FromDateTime(DateTime.Now),
                    ExpectedDeliveryDate = ExpectedDeliveryDate.HasValue
                        ? DateOnly.FromDateTime(ExpectedDeliveryDate.Value)
                        : DateOnly.FromDateTime(DateTime.Now.AddMonths(9)),
                    OrderStatus = "Pending"
                };

                await _wholesaleOrderRepo.AddAsync(newOrder);

                var lines = WholesaleOrderLines.Select(l => new WholesaleOrderLine
                {
                    WholesaleOrderID = newOrder.WholesaleOrderID,
                    ProductID = l.ProductID,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice,
                    QuantityReceived = l.QuantityReceived
                }).ToList();

                await _wholesaleOrderLineRepo.AddRangeAsync(lines);

                // Refresh display view so the new order appears in the bottom list
                _wholesaleView.Refresh();

                // Try to resolve the created order number from the refreshed display list
                int? createdOrderNumber = null;
                var created = WholesaleOrders.FirstOrDefault(x => x.WholesaleOrderID == newOrder.WholesaleOrderID);
                if (created != null && created.WholesaleOrderNumber > 0)
                    createdOrderNumber = created.WholesaleOrderNumber;

                SetFeedbackSuccess(createdOrderNumber);

                WholesaleOrderLines.Clear();
                SelectedProduct = null;
                Quantity = 0;
                ExpectedDeliveryDate = DateTime.Today;

                if (ConfirmCommand is RelayCommand rc)
                    rc.RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                SetFeedbackError(ex.Message);

                if (ConfirmCommand is RelayCommand rc)
                    rc.RaiseCanExecuteChanged();
            }
        }


        public class WholesaleOrderLineEntryViewModel : BaseViewModel
        {
            private Guid _productId;
            public Guid ProductID
            {
                get => _productId;
                set => SetProperty(ref _productId, value);
            }

            private string _productName = string.Empty;
            public string ProductName
            {
                get => _productName;
                set => SetProperty(ref _productName, value);
            }

            private int _quantity;
            public int Quantity
            {
                get => _quantity;
                set => SetProperty(ref _quantity, value);
            }

            private decimal _unitPrice;
            public decimal UnitPrice
            {
                get => _unitPrice;
                set => SetProperty(ref _unitPrice, value);
            }

            private int _quantityReceived;
            public int QuantityReceived
            {
                get => _quantityReceived;
                set => SetProperty(ref _quantityReceived, value);
            }
        }
    }
}
