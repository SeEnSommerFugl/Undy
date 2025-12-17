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
                {
                    CalculatedDifference(AlreadyReceived, ReceivedQuantity, OrderedQuantity);
                    OnPropertyChanged(nameof(Difference));
                }
            }
        }

        public int Difference { get; set; }

        private int CalculatedDifference(int AlreadyReceived, int ReceivedQuantity, int OrderedQuantity)
        {
            int calculated = 0;
            if (AlreadyReceived == 0)
            {
                calculated = OrderedQuantity - ReceivedQuantity;
            }
            else if (AlreadyReceived < OrderedQuantity)
            {
                calculated = OrderedQuantity - AlreadyReceived - ReceivedQuantity;
            }
            else
            {
                calculated = OrderedQuantity - AlreadyReceived;
            }
            return Difference = calculated;
        }
    }
}