using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Undy.Models
{
    internal class PurchaseOrder
    {
        public Guid PurchaseOrderID { get; set; }
        public int ProductNumber { get; set; }
        public int Quantity { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string OrderStatus { get; set; }
        public Guid ProductID { get; set; }
    }
}
