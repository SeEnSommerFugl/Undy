using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Undy.ViewModels
{
    public class IncomingOrderLineViewModel : BaseViewModel
    {
        public Guid WholesaleOrderID { get; set; }
        public Guid ProductID { get; set; }

        public string ProductNumber { get; set; }   
        public string ProductName { get; set; }     

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

        public int Difference => OrderedQuantity - (AlreadyReceived + ReceivedQuantity);
    }
}
