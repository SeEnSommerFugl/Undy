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
        private string _paymentStatus;


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

        // ----- Properties -----

        public string OrderNumber
        {
            get => _orderNumber;
            set { _orderNumber = value; OnPropertyChanged(); }
        }

        public string PaymentStatus
        {
            get => _paymentStatus;
            set
            {
                _paymentStatus = value;
                OnPropertyChanged();
            }
        }

        // Lige nu har SalesOrder ikke et kundenavn – denne kan evt. sættes
        // fra en anden model/lookup.
        public string CustomerName
        {
            get => _customerName;
            set { _customerName = value; OnPropertyChanged(); }
        }

        public decimal TotalAmount
        {
            get => _totalAmount;
            set
            {
                _totalAmount = value;
                OnPropertyChanged();
                PaymentAmount = value; // som standard = total
            }
        }

        public decimal PaymentAmount
        {
            get => _paymentAmount;
            set { _paymentAmount = value; OnPropertyChanged(); }
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
            set { _currentSalesOrderId = value; OnPropertyChanged(); }
        }

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
                               
                order.OrderStatus = SelectedStatus;

                await _salesOrderRepo.UpdateAsync(order);

                CurrentStatus = SelectedStatus;
                PaymentStatus = order.PaymentStatus; // opdater visningen
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

        public void LoadFromSalesOrder(SalesOrder order)
        {
            if (order == null) return;

            CurrentSalesOrderId = order.SalesOrderID;
            OrderNumber = order.OrderNumber.ToString();
            TotalAmount = order.TotalPrice;
            PaymentAmount = order.TotalPrice;    // samme beløb vises som "Betalt beløb"
            CurrentStatus = order.OrderStatus;
            PaymentStatus = order.PaymentStatus; // fx "Betalt"

            CustomerName = string.Empty; // TODO: Der skal kobles kunde på 
            SalesOrderLines.Clear();     // ingen linjer endnu i modellen

            StatusMessage = string.Empty;
        }

    }
}
