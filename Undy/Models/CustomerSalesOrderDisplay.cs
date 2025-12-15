namespace Undy.Models
{
    public class CustomerSalesOrder : INotifyPropertyChanged
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

        private string _fullName;
        public string FullName
        {
            get => _fullName;
            set
            {
                if (_fullName != value)
                {
                    _fullName = value;
                    OnPropertyChanged(nameof(FullName));
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

