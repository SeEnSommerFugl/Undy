namespace Undy.Features.ViewModel
{
    public class IncomingOrderLineViewModel : BaseViewModel
    {
        public Guid WholesaleOrderID { get; set; }
        public Guid ProductID { get; set; }

        public string ProductNumber { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;

        public int OrderedQuantity { get; set; }
        public int AlreadyReceived { get; set; }

        private int _receivedQuantity;
        public int ReceivedQuantity
        {
            get => _receivedQuantity;
            set
            {
                if (SetProperty(ref _receivedQuantity, value))
                    OnPropertyChanged(nameof(Difference));
                }
            }
        }

        
        public int Difference => OrderedQuantity - (AlreadyReceived + ReceivedQuantity);
    }
}