using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Undy.Models;

namespace Undy.ViewModels {
    public class SalesOrderLineViewModel : BaseViewModel {
        public Product Product { get; }
        public int Quantity { get; }
        public decimal UnitPrice => Product.Price;
        public decimal SubTotal => Quantity * UnitPrice;

        public SalesOrderLineViewModel(Product product, int quantity) {
            Product = product;
            Quantity = quantity;
        }
    }
}
