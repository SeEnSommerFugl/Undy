namespace Undy.Features.ViewModel
{
    public class TestReturnOrderViewModel : BaseViewModel
    {
        private readonly IBaseRepository<ReturnOrder, Guid> _tempReturnRepo;
        private readonly IBaseRepository<SalesOrder, Guid> _salesOrderRepo;
        private readonly SalesOrderLineDBRepository _productSalesOrderRepo;

        private string? _orderNumber;
        public string? OrderNumber
        {
            get => _orderNumber;
            set => SetProperty(ref _orderNumber, value);
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string? _reason;
        public string? Reason
        {
            get => _reason;
            set => SetProperty(ref _reason, value);
        }

        private string _orderTotalPrice = string.Empty;
        public string OrderTotalPrice
        {
            get => _orderTotalPrice;
            set => SetProperty(ref _orderTotalPrice, value);
        }

        private decimal _orderTotal;
        public decimal OrderTotal
        {
            get => _orderTotal;
            set => SetProperty(ref _orderTotal, value);
        }

        private string? _statusMessage;
        public string? StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ObservableCollection<SalesOrderLine> OrderLines { get; } = new();

        public ICommand ConfirmCommand { get; }

        public TestReturnOrderViewModel(
            IBaseRepository<ReturnOrder, Guid> tempReturnRepo,
            IBaseRepository<SalesOrder, Guid> salesOrderRepo,
            SalesOrderLineDBRepository productSalesOrderRepo)
        {
            _tempReturnRepo = tempReturnRepo;
            _salesOrderRepo = salesOrderRepo;
            _productSalesOrderRepo = productSalesOrderRepo;

            ConfirmCommand = new RelayCommand(ConfirmAction, CanConfirm);
        }

        private bool CanConfirm(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(OrderNumber) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Reason);
        }

        private async void ConfirmAction(object? parameter)
        {
            StatusMessage = string.Empty;

            // OrderNumber must be the human-readable SalesOrderNumber (INT), not SalesOrderID (GUID)
            if (!int.TryParse(OrderNumber, out int salesOrderNumber))
            {
                StatusMessage = "Ordrenummer er ikke gyldigt. Indtast et tal.";
                return;
            }

            // Lookup SalesOrderID + Customer Email using the SalesOrderNumber
            if (_salesOrderRepo is not SalesOrderDBRepository salesOrderDbRepo)
            {
                StatusMessage = "Intern fejl: SalesOrder repository er ikke korrekt konfigureret.";
                return;
            }

            var (salesOrderId, customerEmail) =
                await salesOrderDbRepo.GetValidationInfoBySalesOrderNumberAsync(salesOrderNumber);

            if (salesOrderId == null)
            {
                StatusMessage = "Ordrenummeret findes ikke";
                return;
            }

            var inputEmail = (Email ?? string.Empty).Trim();
            var dbEmail = (customerEmail ?? string.Empty).Trim();

            if (!string.Equals(inputEmail, dbEmail, StringComparison.OrdinalIgnoreCase))
            {
                StatusMessage = "E-mail stemmer ikke overens med det givne ordrenummer, ret venligst E-Mail addressen";
                return;
            }

            // Load order header for total
            var salesOrder = await _salesOrderRepo.GetByIdAsync(salesOrderId.Value);

            // Load order lines (ProductSalesOrder) for preview ListView
            OrderLines.Clear();
            var orderLines = await _productSalesOrderRepo.GetBySalesOrderIdAsync(salesOrderId.Value);
            foreach (var line in orderLines)
            {
                OrderLines.Add(line);
            }

            OrderTotal = salesOrder.TotalPrice;
            OrderTotalPrice = OrderTotal.ToString("0.00");

            // Create and store the return order (ReturnTotalPrice is snapshotted in SQL on insert)
            var newReturnOrder = new ReturnOrder
            {
                ReturnOrderID = Guid.NewGuid(),
                ReturnOrderDate = DateOnly.FromDateTime(DateTime.Now),
                SalesOrderID = salesOrderId.Value
            };

            await _tempReturnRepo.AddAsync(newReturnOrder);

            StatusMessage = "Returneringen er nu registreret.";

            OrderNumber = string.Empty;
            Email = string.Empty;
            Reason = string.Empty;
            OrderTotalPrice = string.Empty;
            OrderTotal = 0;
            OrderLines.Clear();
        }
    }
}
