using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Undy.Models {
    internal class ProductPurchaseOrder 
    {
        public Guid PurchaseOrderID { get; set; }
        public Guid ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
