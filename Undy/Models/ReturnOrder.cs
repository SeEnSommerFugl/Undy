namespace Undy.Models
{
    public class ReturnOrder
    {
        public Guid ReturnOrderID { get; set; }
        public DateOnly ReturnOrderDate { get; set; }
        public Guid SalesOrderID { get; set; }
    }
}
