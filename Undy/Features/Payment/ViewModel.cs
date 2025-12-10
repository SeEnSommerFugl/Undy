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
        private readonly IBaseRepository<CustomerSalesOrderDisplay, Guid> _customerSalesOrderDisplayRepo;

        // Liste med ordrer der kan betales
        public ObservableCollection<CustomerSalesOrderDisplay> UnpaidOrders { get; }

        private CustomerSalesOrderDisplay _selectedOrder;
        public CustomerSalesOrderDisplay SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged();
                LoadFromSalesOrder(_selectedOrder);
                (ConfirmPaymentCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        // Info om valgt ordre (valgfrit at vise i UI)
        public string OrderNumber { get; private set; }
        public string CustomerName { get; private set; }
        public decimal TotalAmount { get; private set; }

        // Statusbesked (binder til TextBlock nederst)
        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand ConfirmPaymentCommand { get; }

        // HOVED-ctor med repos (bruges fra DI / hvor du vil styre det selv)
        public PaymentViewModel(
            IBaseRepository<SalesOrder, Guid> salesOrderRepo,
            IBaseRepository<CustomerSalesOrderDisplay, Guid> customerSalesOrderDisplayRepo)
        {
            _salesOrderRepo = salesOrderRepo;
            _customerSalesOrderDisplayRepo = customerSalesOrderDisplayRepo;

            UnpaidOrders = new ObservableCollection<CustomerSalesOrderDisplay>();

            ConfirmPaymentCommand = new RelayCommand(
                async _ => await ConfirmPaymentAsync(),
                _ => SelectedOrder != null && SelectedOrder.IsSelectedForPayment
            );
        }

        // Parameterløs ctor – praktisk hvis du bare gør "new PaymentViewModel()" et sted
        public PaymentViewModel()
            : this(new SalesOrderDBRepository(),
                   new CustomerSalesOrderDisplayDBRepository())
        {
        }

        // Loader alle ubetalte ordrer til listen
        public async Task LoadUnpaidOrdersAsync()
        {
            UnpaidOrders.Clear();
            StatusMessage = string.Empty;

            var all = await _customerSalesOrderDisplayRepo.GetAllAsync();

            foreach (var order in all)
            {
                // Kun ubetalte ordrer
                if (!string.Equals(order.PaymentStatus, "Betalt", StringComparison.OrdinalIgnoreCase))
                {
                    order.IsSelectedForPayment = false;
                    UnpaidOrders.Add(order);
                }
            }

            if (UnpaidOrders.Count == 0)
            {
                StatusMessage = "Der er ingen ordrer, der mangler betaling.";
            }
        }

        private void LoadFromSalesOrder(CustomerSalesOrderDisplay order)
        {
            if (order == null)
            {
                OrderNumber = string.Empty;
                CustomerName = string.Empty;
                TotalAmount = 0;
                OnPropertyChanged(nameof(OrderNumber));
                OnPropertyChanged(nameof(CustomerName));
                OnPropertyChanged(nameof(TotalAmount));
                return;
            }

            OrderNumber = order.DisplaySalesOrderNumber;
            CustomerName = order.CustomerName;
            TotalAmount = order.TotalPrice;

            OnPropertyChanged(nameof(OrderNumber));
            OnPropertyChanged(nameof(CustomerName));
            OnPropertyChanged(nameof(TotalAmount));
        }

        private async Task ConfirmPaymentAsync()
        {
            StatusMessage = string.Empty;

            if (SelectedOrder == null)
            {
                StatusMessage = "Vælg en ordre først.";
                return;
            }

            var order = await _salesOrderRepo.GetByIdAsync(SelectedOrder.SalesOrderID);
            if (order == null)
            {
                StatusMessage = "Kunne ikke finde ordren i databasen.";
                return;
            }

            // Registrer betaling
            order.PaymentStatus = "Betalt";

            await _salesOrderRepo.UpdateAsync(order);

            // Opdater UI-modellen
            SelectedOrder.PaymentStatus = "Betalt";
            SelectedOrder.IsSelectedForPayment = false;

            // Fjern ordren fra listen, da den nu er betalt
            UnpaidOrders.Remove(SelectedOrder);
            SelectedOrder = null;

            StatusMessage = "Betaling registreret.";
        }
    }
}
