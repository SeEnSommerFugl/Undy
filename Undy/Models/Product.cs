using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Undy.Models
{
    internal class Product
    {
        public Guid ProductID { get; set; }
        public int ProductNumber { get; set; }
        public string Productname { get; set; } 
        public decimal Price { get; set; }
        public int InStock { get; set; }
        public int Status { get; set; }
        public int MinimumStockLimit { get; set; }
        public Guid ProductCatalogueID { get; set; }


    }
}
