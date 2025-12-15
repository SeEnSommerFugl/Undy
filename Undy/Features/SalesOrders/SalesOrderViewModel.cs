namespace Undy.Features.ViewModel
{
    public class SalesOrderViewModel : BaseViewModel
    {
        // Repository for sales order display rows
        private readonly IBaseRepository<SalesOrder, Guid> _salesOrderRepo;

        // Repository for product lines on sales orders
        private readonly IBaseRepository<ProductSalesOrder, Guid> _productSalesOrderRepo;

        // All sales orders for the ListView
        public ObservableCollection<SalesOrder> SalesDisplay => _salesOrderRepo.Items;

        // View for sorting and filtering
        public ICollectionView SaleView { get; }

        // Lines for the selected order (shown in the DataGrid)
        public ObservableCollection<ProductSalesOrder> SelectedOrderDetails { get; }
            = new ObservableCollection<ProductSalesOrder>();

        // Order status options used in the view
        public List<string> OrderStatusOptions { get; } = new() { "Afventer", "Afsendt" };

        // 
        // SaleView binds directly to Repository
        public SalesOrderViewModel(
            IBaseRepository<SalesOrder, Guid> salesDisplayRepo,
            IBaseRepository<ProductSalesOrder, Guid> productSalesOrderRepo)
        {
            _salesOrderRepo = salesDisplayRepo;
            _productSalesOrderRepo = productSalesOrderRepo;

            SaleView = CollectionViewSource.GetDefaultView(SalesDisplay);
            SaleView.SortDescriptions.Add(new SortDescription("SalesDate", ListSortDirection.Descending));
        }

        // SelectedOrderDetails is a separate collection
        // LoadOrderDetails filters lines with SalesOrderID
        public void LoadOrderDetails(SalesOrder selectedOrder)
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
