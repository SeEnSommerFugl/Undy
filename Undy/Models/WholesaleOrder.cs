namespace Undy.Models
{
    public class WholesaleOrder
    {
        public Guid PurchaseOrderID { get; set; }
        public DateOnly ExpectedDeliveryDate { get; set; }
        public DateOnly OrderDate { get; set; }
        public DateOnly DeliveryDate { get; set; }
        public string OrderStatus { get; set; }
        public Guid ProductID { get; set; }
        public int OrderedQuantity { get; set; }
        public int ReceivedQuantity { get; set; }
    }
}
