using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Undy.Models
{
    public class TestWholesaleOrder
    {
        public Guid PurchaseOrderID { get; set; }
        public DateOnly ExpectedDeliveryDate { get; set; }
        public DateOnly OrderDate { get; set; }
        public DateOnly DeliveryDate { get; set; }
        public string OrderStatus { get; set; }
        public Guid ProductID { get; set; }
    }
}
