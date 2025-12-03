using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Undy.Data.Repository;
using Undy.Models;


namespace Undy.ViewModels
{
    public class PaymentViewModel : BaseViewModel
    {
        private readonly IBaseRepository<SalesOrder, Guid> _salesOrderRepo;

        // ----- backing fields -----

        private string _orderNumber;
        private string _customerName;
        private decimal _totalAmount;
        private decimal _paymentAmount;

        private ObservableCollection<SalesOrderLineViewModel> _orderLines;

        private ObservableCollection<string> _paymentMethods;
        private string _selectedPaymentMethod;

        private ObservableCollection<string> _statusOptions;
        private string _selectedStatus;

        private string _statusMessage;

        private Guid _currentSalesOrderId;

        // ----- Constructor -----

        public PaymentViewModel(IBaseRepository<SalesOrder, Guid> salesOrderRepo)
        {
            _salesOrderRepo = salesOrderRepo;

            OrderLines = new ObservableCollection<SalesOrderLineViewModel>();

            // 
            PaymentMethods = new ObservableCollection<string>
            {
                "Kort",
                "MobilePay",
                "Kontant",
                "Bankoverførsel"
            };

            StatusOptions = new ObservableCollection<string>
            {
                "Ny",
                "Under behandling",
                "Afventer betaling",
                "Betalt",
                "Annulleret",
                "Returneret"
            };

            ConfirmPaymentCommand = new RelayCommand(
                execute: _ => ConfirmPayment(),
                canExecute: _ => CanConfirmPayment()
            );
        }

       
}
