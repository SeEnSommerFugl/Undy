namespace Undy.Features.ViewModel
{
    public class PaymentViewModel : BaseViewModel
    {
        private readonly IBaseRepository<SalesOrder, Guid> _salesOrderRepo;
        private readonly IBaseRepository<CustomerSalesOrderDisplay, Guid> _customerSalesOrderDisplayRepo;

        // Liste med ALLE ordrer 
        public ObservableCollection<CustomerSalesOrderDisplay> Orders => _customerSalesOrderDisplayRepo.Items;

        // Viewet som UI binder til
        public ICollectionView PaymentView { get; }

        private CustomerSalesOrderDisplay _selectedOrder;
        public CustomerSalesOrderDisplay SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged();
                LoadSelectedOrderInfo(_selectedOrder);

                (ConfirmPaymentCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        // Info om valgt ordre 
        public string OrderNumber { get; private set; }
        public string CustomerName { get; private set; }
        public decimal TotalAmount { get; private set; }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public ICommand ConfirmPaymentCommand { get; }

        
        // Constructors
        public PaymentViewModel(
            IBaseRepository<SalesOrder, Guid> salesOrderRepo,
            IBaseRepository<CustomerSalesOrderDisplay, Guid> customerSalesOrderDisplayRepo)
        {
            _salesOrderRepo = salesOrderRepo;
            _customerSalesOrderDisplayRepo = customerSalesOrderDisplayRepo;

            PaymentView = CollectionViewSource.GetDefaultView(Orders);
            PaymentView.SortDescriptions.Add(new SortDescription("SalesDate", ListSortDirection.Descending));
            PaymentView.Filter = po => po is CustomerSalesOrderDisplay paymentOrder && paymentOrder.PaymentStatus != "Betalt";

            ConfirmPaymentCommand = new RelayCommand(
                async _ => await ConfirmPaymentAsync(),
                _ => SelectedOrder != null && SelectedOrder.IsSelectedForPayment
            );
        }
        
        // Refresh 
        private void RefreshPaymentView()
        {
            PaymentView.Refresh();
        }

        
        // Vis info om valgt ordre
        private void LoadSelectedOrderInfo(CustomerSalesOrderDisplay order)
        {
            if (order == null)
            {
                OrderNumber = string.Empty;
                CustomerName = string.Empty;
                TotalAmount = 0;
            }
            else
            {
                OrderNumber = order.DisplaySalesOrderNumber;
                CustomerName = order.FullName;
                TotalAmount = order.TotalPrice;
            }

            OnPropertyChanged(nameof(OrderNumber));
            OnPropertyChanged(nameof(CustomerName));
            OnPropertyChanged(nameof(TotalAmount));
        }

        
        // Registrér betaling
        private async Task ConfirmPaymentAsync()
        {
            StatusMessage = string.Empty;

            if (SelectedOrder == null)
            {
                StatusMessage = "Vælg en ordre først.";
                return;
            }

            var dbOrder = await _salesOrderRepo.GetByIdAsync(SelectedOrder.SalesOrderID);

            if (dbOrder == null)
            {
                StatusMessage = "Kunne ikke finde ordren i databasen.";
                return;
            }

            
            dbOrder.PaymentStatus = "Betalt";
            await _salesOrderRepo.UpdateAsync(dbOrder);

            
            SelectedOrder.PaymentStatus = "Betalt";
            SelectedOrder.IsSelectedForPayment = false;

            
            RefreshPaymentView();

            StatusMessage = "Betaling registreret.";
        }
    }
}
