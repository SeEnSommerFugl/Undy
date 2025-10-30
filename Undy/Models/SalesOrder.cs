namespace Undy.Models
{
    public class SalesOrder
    {
        public Guid SalesOrderID { get; set; }
        public int ProductNumber { get; set; }
        public int Quantity { get; set; }
        public string SalesOrderStatus { get; set; }
        public Guid ProductID { get; set; }
    }
}
