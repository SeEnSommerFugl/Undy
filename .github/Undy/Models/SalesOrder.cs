using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Undy.Models
{
    internal class SalesOrder
    {
        public Guid SalesOrderID { get; set; }
        public int ProductNumber { get; set; }
        public int Quantity { get; set; }
        public string SalesOrderStatus { get; set; }
        public Guid ProductID { get; set; }
    }
}
