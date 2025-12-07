using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Undy.Models
{
    public class WholeSaleOrderLine
    {
        public Guid WholesaleOrderLineID { get; set; }

        public Guid PurchaseOrderID { get; set; }   
        public Guid ProductID { get; set; }        
        public int OrderedQuantity { get; set; }    
        public int ReceivedQuantity { get; set; }
    }
}
