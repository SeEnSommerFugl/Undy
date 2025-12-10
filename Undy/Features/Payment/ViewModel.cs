using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels
{
    public class PaymentViewModel : BaseViewModel
    {
        private readonly IBaseRepository<SalesOrder, Guid> _salesOrderRepo;

        private string _orderNumber;
        private string _customerName;
        private decimal _totalAmount;
        private decimal _paymentAmount;

        private ObservableCollection<SalesOrderLineViewModel> _salesOrderLines;

        private ObservableCollection<string> _statusOptions;
        private string _selectedStatus;
        private string _statusMessage;
        private string _currentStatus;

        private Guid _currentSalesOrderId;

        // ----- Constructor -----

        public PaymentViewModel(IBaseRepository<SalesOrder, Guid> salesOrderRepo)
        {
            _salesOrderRepo = salesOrderRepo;

            SalesOrderLines = new ObservableCollection<SalesOrderLineViewModel>();
            StatusOptions = new ObservableCollection<string>();

            ConfirmPaymentCommand = new RelayCommand(
                async _ => await ConfirmPaymentAsync(),
                _ => CanConfirmPayment()
            );
        }

        // ----- Core properties (data) -----

        public string OrderNumber
        {
            get => _orderNumber;
            set
            {
                _orderNumber = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayOrderNumber));
            }
        }

        public string CustomerName
        {
            get => _customerName;
            set
            {
                _customerName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayCustomerName));
            }
        }

        public decimal TotalAmount
        {
            get => _totalAmount;
            set
            {
                _totalAmount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayTotalAmount));
                // Vis som standard samme beløb som betaling
                PaymentAmount = value;
            }
        }

        public decimal PaymentAmount
        {
            get => _paymentAmount;
            set
            {
                _paymentAmount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayPaymentAmount));
            }
        }

        public ObservableCollection<SalesOrderLineViewModel> SalesOrderLines
        {
            get => _salesOrderLines;
            set { _salesOrderLines = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> StatusOptions
        {
            get => _statusOptions;
            set { _statusOptions = value; OnPropertyChanged(); }
        }

        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                _selectedStatus = value;
                OnPropertyChanged();
                RaiseConfirmCanExecuteChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        /// <summary>Aktuel OrderStatus fra databasen.</summary>
        public string CurrentStatus
        {
            get => _currentStatus;
            set
            {
                _currentStatus = value;
                OnPropertyChanged();
                UpdateStatusOptions();
            }
        }

        public Guid CurrentSalesOrderId
        {
            get => _currentSalesOrderId;
            set
            {
                _currentSalesOrderId = value;
                OnPropertyChanged();

                // når ID ændres, ændrer placeholders sig også
                OnPropertyChanged(nameof(DisplayOrderNumber));
                OnPropertyChanged(nameof(DisplayCustomerName));
                OnPropertyChanged(nameof(DisplayTotalAmount));
                OnPropertyChanged(nameof(DisplayPaymentAmount));
            }
        }

        // ----- Display properties (til UI / placeholders) SCOPE? -----

        public string DisplayOrderNumber =>
            CurrentSalesOrderId == Guid.Empty
                ? "Ingen ordre valgt"
                : OrderNumber;

        public string DisplayCustomerName =>
            CurrentSalesOrderId == Guid.Empty
                ? "Ingen ordre valgt"
                : CustomerName;

        public string DisplayTotalAmount =>
            CurrentSalesOrderId == Guid.Empty
                ? "Ingen ordre valgt"
                : TotalAmount.ToString("0.00");

        public string DisplayPaymentAmount =>
            CurrentSalesOrderId == Guid.Empty
                ? "Ingen ordre valgt"
                : PaymentAmount.ToString("0.00");

        // ----- Command -----

        public ICommand ConfirmPaymentCommand { get; }

        private bool CanConfirmPayment()
        {
            // Krav: der skal være valgt en status
            return !string.IsNullOrWhiteSpace(SelectedStatus);
        }

        private async Task ConfirmPaymentAsync()
        {
            try
            {
                var order = await _salesOrderRepo.GetByIdAsync(CurrentSalesOrderId);
                if (order == null)
                {
                    StatusMessage = "Kunne ikke finde ordren.";
                    return;
                }

                // UC-03: vi ændrer kun ordrestatus her
                order.OrderStatus = SelectedStatus;

                await _salesOrderRepo.UpdateAsync(order);

                CurrentStatus = SelectedStatus;
                SelectedStatus = null;
                StatusMessage = "Ordren er behandlet.";
            }
            catch
            {
                StatusMessage = "Der opstod en fejl ved behandling af ordren.";
            }
        }

        private void RaiseConfirmCanExecuteChanged()
        {
            (ConfirmPaymentCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        
        private void UpdateStatusOptions()
        {
            StatusOptions.Clear();
            SelectedStatus = null;

            switch (CurrentStatus)
            {
                case "Under behandling":
                    StatusOptions.Add("Klar til afsendelse");
                    StatusOptions.Add("Returneret");
                    break;

                case "Klar til afsendelse":
                    StatusOptions.Add("Afsendt");
                    StatusOptions.Add("Returneret");
                    break;

                case "Afsendt":
                    // typisk færdig – evt. kun "Returneret"
                    StatusOptions.Add("Returneret");
                    break;

                default:
                    // Fallback, hvis status er ukendt
                    StatusOptions.Add("Under behandling");
                    StatusOptions.Add("Klar til afsendelse");
                    StatusOptions.Add("Afsendt");
                    StatusOptions.Add("Returneret");
                    break;
            }

            OnPropertyChanged(nameof(StatusOptions));
        }

        // ----- Loader -----

        public void LoadFromSalesOrder(SalesOrderDisplay order)
        {
            if (order == null) return;

            CurrentSalesOrderId = order.SalesOrderID;
            OrderNumber = order.SalesOrderNumber.ToString();
            TotalAmount = order.TotalPrice;   // sætter også PaymentAmount
            CurrentStatus = order.OrderStatus;

            // TODO: Der skal kobles kunde på senere?
            CustomerName = string.Empty;

            SalesOrderLines.Clear();  // kan senere fyldes med rigtige linjer

            StatusMessage = string.Empty;
        }
    }
}
