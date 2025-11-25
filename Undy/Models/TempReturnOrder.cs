using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Undy.Models
{
    public class TestReturnOrder
    {
        public Guid ReturnOrderID { get; set; }
        public DateOnly ReturnOrderDate { get; set; }
        public Guid SalesOrderID { get; set; }

    }
}
