namespace Undy.Models
{
    public class WholesaleOrder {
        public Guid PurchaseOrderID { get; set; }
        public DateOnly ExpectedDeliveryDate { get; set; }
        public DateOnly OrderDate { get; set; }
        public DateOnly? DeliveryDate { get; set; }
        public string OrderStatus { get; set; }
        public int PurchaseOrderNumber { get; set; }
        public DateOnly PurchaseOrderDate { get; set; }
        public string DisplayPurchaseOrderNumber { get; set; }

    }
}
