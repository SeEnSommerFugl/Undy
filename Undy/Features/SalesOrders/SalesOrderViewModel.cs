using System.Collections.ObjectModel;

namespace Undy.Features.ViewModel
{
    /// <summary>
    /// ViewModel for displaying existing sales orders and their lines.
    /// Creation/editing is handled in TestSalesOrderViewModel.
    /// </summary>
    public class SalesOrderViewModel : BaseViewModel
    {
        private readonly IBaseRepository<SalesOrder, Guid> _salesOrderRepo;
        private readonly SalesOrderLineDBRepository _salesOrderLineRepo;

        private SalesOrder? _selectedSalesOrder;

        public SalesOrderViewModel(
            IBaseRepository<SalesOrder, Guid> salesOrderRepo,
            SalesOrderLineDBRepository salesOrderLineRepo)
        {
            _salesOrderRepo = salesOrderRepo;
            _salesOrderLineRepo = salesOrderLineRepo;

            SelectedOrderLines = new ObservableCollection<SalesOrderLine>();
            SelectedOrderDetails = new ObservableCollection<SalesOrderDetailRow>();
        }

        /// <summary>
        /// Backed by SalesOrderDBRepository.Items (loaded in App.xaml.cs).
        /// </summary>
        public ObservableCollection<SalesOrder> SalesOrders => _salesOrderRepo.Items;

        public SalesOrder? SelectedSalesOrder
        {
            get => _selectedSalesOrder;
            set
            {
                if (_selectedSalesOrder == value)
                    return;

                _selectedSalesOrder = value;
                OnPropertyChanged();

                SelectedOrderDetails.Clear();

                if (_selectedSalesOrder != null)
                {
                    SelectedOrderDetails.Add(new SalesOrderDetailRow("Ordrenummer:", _selectedSalesOrder.SalesOrderNumber));
                    SelectedOrderDetails.Add(new SalesOrderDetailRow("E-Mail:", _selectedSalesOrder.CustomerEmail));
                    SelectedOrderDetails.Add(new SalesOrderDetailRow("Købsdato:", _selectedSalesOrder.SalesDate));
                    SelectedOrderDetails.Add(new SalesOrderDetailRow("Afsendt:", _selectedSalesOrder.ShippedDate));
                    SelectedOrderDetails.Add(new SalesOrderDetailRow("Total pris:", _selectedSalesOrder.TotalPrice));
                    SelectedOrderDetails.Add(new SalesOrderDetailRow("Kundenummer:", _selectedSalesOrder.CustomerNumber));
                }
            }
        }

        /// <summary>
        /// Rows for the Selected Order Details ListView.
        /// </summary>
        public ObservableCollection<SalesOrderDetailRow> SelectedOrderDetails { get; }

        /// <summary>
        /// Lines for the currently selected order.
        /// Loaded explicitly (double-click in the view).
        /// </summary>
        public ObservableCollection<SalesOrderLine> SelectedOrderLines { get; }

        /// <summary>
        /// Loads order lines for a specific sales order.
        /// </summary>
        public async Task LoadOrderDetailsAsync(SalesOrder order)
        {
            SelectedOrderLines.Clear();

            var lines = await _salesOrderLineRepo.GetBySalesOrderIdAsync(order.SalesOrderID);
            foreach (var line in lines)
            {
                SelectedOrderLines.Add(line);
            }
        }
    }

    public sealed class SalesOrderDetailRow
    {
        public string Label { get; }
        public object? Value { get; }

        public SalesOrderDetailRow(string label, object? value)
        {
            Label = label;
            Value = value;
        }
    }
}
