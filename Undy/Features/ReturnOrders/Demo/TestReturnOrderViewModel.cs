namespace Undy.Features.ViewModel
{
    public class TestReturnOrderViewModel : BaseViewModel
    {
        private IBaseRepository<ReturnOrder, Guid> _tempReturnRepo;
        private IBaseRepository<SalesOrder, Guid> _salesOrderRepo;

        private string? _orderNumber;
        public string? OrderNumber
        {
            get => _orderNumber;
            set
            {
                if (SetProperty(ref _orderNumber, value)) ;
            }
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value)) ;
            }
        }

        private string? _reason;
        public string? Reason
        {
            get => _reason;
            set
            {
                if (SetProperty(ref _reason, value)) ;
            }
        }

        private string? _statusMessage;
        public string? StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ICommand ConfirmCommand { get; }

        public TestReturnOrderViewModel(IBaseRepository<ReturnOrder, Guid> tempReturnRepo, IBaseRepository<SalesOrder, Guid> salesOrderRepo)
        {
            _tempReturnRepo = tempReturnRepo;
            _salesOrderRepo = salesOrderRepo;

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

            var (salesOrderId, customerEmail) = await salesOrderDbRepo.GetValidationInfoBySalesOrderNumberAsync(salesOrderNumber);

            // If order number does not exist
            if (salesOrderId == null)
            {
                StatusMessage = "Ordrenummeret findes ikke";
                return;
            }

            // If email does not match the order's customer email
            var inputEmail = (Email ?? string.Empty).Trim();
            var dbEmail = (customerEmail ?? string.Empty).Trim();

            if (!string.Equals(inputEmail, dbEmail, StringComparison.OrdinalIgnoreCase))
            {
                StatusMessage = "E-mail stemmer ikke overens med det givne ordrenummer, ret venligst E-Mail addressen";
                return;
            }

            // Create and store the return order (stores SalesOrderID GUID as FK)
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
        }
    }
}
