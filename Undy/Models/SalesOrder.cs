namespace Undy.Models
{
    public class SalesOrder
    {
        public Guid SalesOrderID { get; set; }
        public int OrderNumber { get; set; }
        public string SalesOrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public DateOnly SalesDate { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
