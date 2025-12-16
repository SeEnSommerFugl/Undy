namespace Undy.Models
{
    public class SalesOrderLine
    {
        public Guid SalesOrderID { get; set; }
        public Guid ProductID { get; set; }
        public string ProductNumber { get; set; }
        public string? ProductName { get; set; }

        public int SalesOrderNumber { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => Quantity * UnitPrice;

    }
}
