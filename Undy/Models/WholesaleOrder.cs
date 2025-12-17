namespace Undy.Models
{
    public class WholesaleOrder
    {
        public Guid WholesaleOrderID { get; set; }
        public DateOnly ExpectedDeliveryDate { get; set; }
        public DateOnly WholesaleOrderDate { get; set; }
        public DateOnly? DeliveryDate { get; set; }
        public string OrderStatus { get; set; }
        public int WholesaleOrderNumber { get; set; }
        public string DisplayWholesaleOrderNumber { get; set; }

    }
}
