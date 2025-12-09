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
        public decimal TotalPrice { get; set; }
        public string DisplaySalesOrderNumber {  get; set; }
        public int CustomerNumber { get; set; }
    }
}
