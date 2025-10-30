namespace Undy.Models
{
    public class PurchaseOrder
    {
        public Guid PurchaseOrderID { get; set; }
        public int ProductNumber { get; set; }
        public int Quantity { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string OrderStatus { get; set; }
        public Guid ProductID { get; set; }
    }
}
