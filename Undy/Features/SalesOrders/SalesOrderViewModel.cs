namespace Undy.Features.ViewModel
{
    public class SalesOrderViewModel : BaseViewModel
    {
        // Repository for sales order display rows
        private readonly IBaseRepository<SalesOrderDisplay, Guid> _salesDisplayRepo;

        // Repository for product lines on sales orders
        private readonly IBaseRepository<ProductSalesOrder, Guid> _productSalesOrderRepo;

        // All sales orders for the ListView
        public ObservableCollection<SalesOrderDisplay> SalesDisplay => _salesDisplayRepo.Items;

        // View for sorting and filtering
        public ICollectionView SaleView { get; }

        // Lines for the selected order (shown in the DataGrid)
        public ObservableCollection<ProductSalesOrder> SelectedOrderDetails { get; }
            = new ObservableCollection<ProductSalesOrder>();

        // Order status options used in the view
        public List<string> OrderStatusOptions { get; } = new() { "Afventer", "Under Behandling", "Afsendt" };

        public SalesOrderViewModel(
            IBaseRepository<SalesOrderDisplay, Guid> salesDisplayRepo,
            IBaseRepository<ProductSalesOrder, Guid> productSalesOrderRepo)
        {
            _salesDisplayRepo = salesDisplayRepo;
            _productSalesOrderRepo = productSalesOrderRepo;

            SaleView = CollectionViewSource.GetDefaultView(SalesDisplay);
            SaleView.SortDescriptions.Add(new SortDescription("SalesDate", ListSortDirection.Descending));

            foreach (var salesOrder in SalesDisplay)
            {
                salesOrder.PropertyChanged += salesOrder_PropertyChanged;
            }
        }

        private void salesOrder_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is SalesOrderDisplay salesOrder)
            {
                if (e.PropertyName == nameof(salesOrder.OrderStatus))
                {
                    // Save updated status to the database
                    _salesDisplayRepo.UpdateAsync(salesOrder);
                }
            }
        }

        public void LoadOrderDetails(SalesOrderDisplay selectedOrder)
        {
            // Clear old lines
            SelectedOrderDetails.Clear();

            if (selectedOrder == null)
            {
                return;
            }

            // Find all product lines for this order
            var lines = _productSalesOrderRepo.Items
                .Where(l => l.SalesOrderID == selectedOrder.SalesOrderID);

            foreach (var line in lines)
            {
                SelectedOrderDetails.Add(line);
            }
        }
    }
}
