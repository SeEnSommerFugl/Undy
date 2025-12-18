// Undy/Undy/Features/WholesaleOrders/ArrivedOrder/IncomingWholesaleOrderViewModel.cs
namespace Undy.Features.ViewModel
{
    public class IncomingWholesaleOrderViewModel : BaseViewModel
    {
        private readonly IBaseRepository<WholesaleOrder, Guid> _wholesaleOrderRepo;
        private readonly IBaseRepository<Product, Guid> _productRepo;
        private readonly WholesaleOrderLineDBRepository _wholesaleOrderLineRepo;

        // Snapshot of QuantityReceived as loaded from DB (needed to compute ReceiveQuantity delta)
        private readonly Dictionary<WholesaleOrderLine.WholesaleOrderLineKey, int> _originalReceivedByKey = new();

        public ObservableCollection<WholesaleOrder> WholesaleOrders => _wholesaleOrderRepo.Items;

        public ICollectionView WholesaleView { get; }
        public ICollectionView LinesView { get; }

        public ObservableCollection<WholesaleOrderLine> SelectedOrderLines { get; }

        public ICommand ConfirmOrderCommand { get; }

        private bool _isFullyReceived;
        public bool IsFullyReceived
        {
            get => _isFullyReceived;
            set
            {
                if (SetProperty(ref _isFullyReceived, value))
                {
                    if (value)
                    {
                        // Fill remaining on all lines (so user can confirm in one click)
                        foreach (var line in SelectedOrderLines)
                        {
                            line.QuantityReceived = line.Quantity; // clamped in model
                        }
                    }

                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public IncomingWholesaleOrderViewModel(
            IBaseRepository<WholesaleOrder, Guid> wholesaleOrderRepo,
            IBaseRepository<Product, Guid> productRepo,
            WholesaleOrderLineDBRepository wholesaleOrderLineRepo)
        {
            _wholesaleOrderRepo = wholesaleOrderRepo;
            _productRepo = productRepo;
            _wholesaleOrderLineRepo = wholesaleOrderLineRepo;

            WholesaleView = CollectionViewSource.GetDefaultView(WholesaleOrders);
            WholesaleView.SortDescriptions.Clear();
            WholesaleView.SortDescriptions.Add(
                new SortDescription(nameof(WholesaleOrder.WholesaleOrderNumber), ListSortDirection.Descending));


            SelectedOrderLines = new ObservableCollection<WholesaleOrderLine>();
            LinesView = CollectionViewSource.GetDefaultView(SelectedOrderLines);

            ConfirmOrderCommand = new RelayCommand(_ => ConfirmReceiptAsync(), _ => CanConfirmReceipt());
        }

        private WholesaleOrder? _selectedWholesaleOrder;
        public WholesaleOrder? SelectedWholesaleOrder
        {
            get => _selectedWholesaleOrder;
            set
            {
                if (SetProperty(ref _selectedWholesaleOrder, value))
                {
                    IsFullyReceived = false;
                    _ = LoadSelectedOrderLinesAsync();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private async Task LoadSelectedOrderLinesAsync()
        {
            SelectedOrderLines.Clear();
            _originalReceivedByKey.Clear();

            if (SelectedWholesaleOrder is null)
                return;

            var lines = await _wholesaleOrderLineRepo.GetByIdsAsync([SelectedWholesaleOrder.WholesaleOrderID]);

            foreach (var line in lines)
            {
                SelectedOrderLines.Add(line);
                _originalReceivedByKey[line.Key] = line.QuantityReceived;
            }

            CommandManager.InvalidateRequerySuggested();
        }

        private bool CanConfirmReceipt()
        {
            if (SelectedWholesaleOrder is null) return false;
            if (SelectedOrderLines.Count == 0) return false;

            // Enable only if there's at least one positive delta to receive
            foreach (var line in SelectedOrderLines)
            {
                var original = _originalReceivedByKey.TryGetValue(line.Key, out var v) ? v : 0;
                var delta = line.QuantityReceived - original;
                if (delta > 0) return true;
            }

            return false;
        }

        private async void ConfirmReceiptAsync()
        {
            if (SelectedWholesaleOrder is null)
                return;

            try
            {
                var receipts = new List<(Guid WholesaleOrderID, Guid ProductID, int ReceiveQuantity)>();

                foreach (var line in SelectedOrderLines)
                {
                    var original = _originalReceivedByKey.TryGetValue(line.Key, out var v) ? v : 0;
                    var delta = line.QuantityReceived - original;

                    if (delta < 0)
                    {
                        MessageBox.Show(
                            $"Du kan ikke reducere 'Modtaget'.\n\n{line.ProductNumber} - {line.ProductName}\nOriginal: {original}\nNu: {line.QuantityReceived}",
                            "Ugyldig modtagelse",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }

                    if (delta == 0)
                        continue;

                    receipts.Add((line.WholesaleOrderID, line.ProductID, delta));
                }

                if (receipts.Count == 0)
                {
                    MessageBox.Show("Der er ingen ændringer at gemme.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Capture selected order id before we reload anything
                var selectedId = SelectedWholesaleOrder.WholesaleOrderID;

                // Business logic proc:
                // - increments QuantityReceived
                // - increments Product.NumberInStock
                // - updates WholesaleOrder status + DeliveryDate when fully received
                await _wholesaleOrderLineRepo.ProcessReceiptLinesAsync(receipts);

                // Reload lines (DB truth)
                await LoadSelectedOrderLinesAsync();

                // Reload wholesale orders so OrderStatus/DeliveryDate updates in the left list
                await _wholesaleOrderRepo.InitializeAsync();



                // Re-select the same order object from the refreshed collection (so UI shows new status)
                SelectedWholesaleOrder = WholesaleOrders.FirstOrDefault(o => o.WholesaleOrderID == selectedId);
                WholesaleView.MoveCurrentTo(SelectedWholesaleOrder);
                WholesaleView.Refresh();

                MessageBox.Show("Modtagelse gemt.", "OK", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (SqlException ex)
            {
                // RAISERROR messages end up here
                MessageBox.Show(ex.Message, "SQL-fejl", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Fejl", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }
}
