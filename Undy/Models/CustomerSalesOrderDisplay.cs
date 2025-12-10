namespace Undy.Models
{
    public class CustomerSalesOrderDisplay : INotifyPropertyChanged
    {
        // Nøgle fra SalesOrder
        public Guid SalesOrderID { get; set; }

        public int SalesOrderNumber { get; set; }
        public string DisplaySalesOrderNumber { get; set; }

        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public DateOnly SalesDate { get; set; }
        public decimal TotalPrice { get; set; }

        // Kunde-info
        public Guid CustomerID { get; set; }
        public int CustomerNumber { get; set; }
        public string DisplayCustomerNumber { get; set; }

        private string _customerName;
        public string CustomerName
        {
            get => _customerName;
            set
            {
                if (_customerName != value)
                {
                    _customerName = value;
                    OnPropertyChanged(nameof(CustomerName));
                }
            }
        }

        // UI: markeret til betaling
        private bool _isSelectedForPayment;
        public bool IsSelectedForPayment
        {
            get => _isSelectedForPayment;
            set
            {
                if (_isSelectedForPayment != value)
                {
                    _isSelectedForPayment = value;
                    OnPropertyChanged(nameof(IsSelectedForPayment));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

