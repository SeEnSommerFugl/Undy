namespace Undy.Models
{
    public class SalesOrderDisplay
    {
        //Properties for SalesOrder
        public Guid SalesOrderID { get; set; }
        public int OrderNumber { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public DateOnly SalesDate { get; set; }
        public decimal TotalPrice { get; set; }

        //Properties for Product
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
}
