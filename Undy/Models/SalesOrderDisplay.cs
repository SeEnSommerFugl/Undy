namespace Undy.Models
{
    public class SalesOrderDisplay : INotifyPropertyChanged
    {
        //Properties for SalesOrder
        public Guid SalesOrderID { get; set; }
        public int SalesOrderNumber { get; set; }
        public string PaymentStatus { get; set; }
        public DateOnly SalesDate { get; set; }
        public decimal TotalPrice { get; set; }
        private string _orderStatus;
        public string OrderStatus
        {
            get => _orderStatus;
            set
            {
                if (_orderStatus != value)
                {
                    _orderStatus = value;
                    OnPropertyChanged(nameof(OrderStatus));
                }
            }
        }
        //Properties for Product
        public string ProductName { get; set; }
        public int Quantity { get; set; }

        //Implementing inherited PropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
