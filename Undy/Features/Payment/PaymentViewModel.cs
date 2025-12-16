namespace Undy.Features.ViewModel
{
    public class PaymentViewModel : BaseViewModel
    {
        private readonly IBaseRepository<SalesOrder, Guid> _salesOrderRepo;
        private readonly IBaseRepository<CustomerSalesOrderDisplay, Guid> _customerSalesOrderRepo;

        // Liste med ALLE ordrer 
        public ObservableCollection<CustomerSalesOrderDisplay> Orders => _customerSalesOrderRepo.Items;

        // Viewet som UI binder til
        public ICollectionView PaymentView { get; }

        public string OrderNumber { get; private set; }
        public string CustomerName { get; private set; }
        public decimal TotalAmount { get; private set; }

        //Order line selections using hashset
        private HashSet<Guid> _selectedSalesOrderIds = new HashSet<Guid>();

        // Info om valgt ordre 
        private CustomerSalesOrderDisplay _selectedOrder;
        public CustomerSalesOrderDisplay SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                SetProperty(ref _selectedOrder, value);
            }
        }

        private bool _isSelectedForPayment;
        public bool IsSelectedForPayment {
            get => _isSelectedForPayment;
            set {
                SetProperty(ref _isSelectedForPayment, value);
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set {
                SetProperty(ref _statusMessage, value);
            }
        }

        public ICommand ConfirmPaymentCommand { get; }

        // Constructor
        public PaymentViewModel(
            IBaseRepository<SalesOrder, Guid> salesOrderRepo,
            IBaseRepository<CustomerSalesOrderDisplay, Guid> customerSalesOrderRepo)
        {
            _salesOrderRepo = salesOrderRepo;
            _customerSalesOrderRepo = customerSalesOrderRepo;

            PaymentView = CollectionViewSource.GetDefaultView(Orders);
            PaymentView.SortDescriptions.Add(new SortDescription("SalesDate", ListSortDirection.Descending));
            PaymentView.Filter = po => po is CustomerSalesOrderDisplay paymentOrder && paymentOrder.PaymentStatus != "Betalt";

            ConfirmPaymentCommand = new RelayCommand(
                async _ => await ConfirmPaymentAsync(),
                _ => _selectedSalesOrderIds.Any()
            );

            //ConfirmPaymentCommand = new RelayCommand(
            //    async _ => await ConfirmPaymentAsync(),
            //    _ => SelectedOrder != null && SelectedOrder.IsSelectedForPayment
            //);
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

        public bool IsSalesOrderSelected(Guid SalesOrderID) => _selectedSalesOrderIds.Contains(SalesOrderID);

        public void SetSalesOrderSelection(Guid SalesOrderID, bool IsSelectedForPayment) {
            if(IsSelectedForPayment) {
                _selectedSalesOrderIds.Add(SalesOrderID);
            } else {
                _selectedSalesOrderIds.Remove(SalesOrderID);
            }
        }
    }
}
