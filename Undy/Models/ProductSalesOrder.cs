namespace Undy.Models
{
    public class ProductSalesOrder
    {
        public Guid SalesOrderID { get; set; }
        public Guid ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
