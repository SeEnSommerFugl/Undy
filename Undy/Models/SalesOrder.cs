namespace Undy.Models
{
    public class SalesOrder
    {
        public Guid SalesOrderID { get; set; }
        public Guid CustomerID { get; set; }
        public int SalesOrderNumber { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public DateOnly SalesDate { get; set; }
        public DateOnly? ShippedDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string DisplaySalesOrderNumber { get; set; }

        // Comes from View/Join - For display purposes only
        public string? CustomerName { get; set; }
        public string? City { get; set; }
    }
}
