namespace Undy.Models
{
    public class WholesaleOrderLine

    {
        public Guid WholesaleOrderID { get; set; }
        public Guid ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int QuantityReceived { get; set; }
        public WholesaleOrderLineKey Key => new(WholesaleOrderID, ProductID);
        public readonly record struct WholesaleOrderLineKey(Guid WholesaleOrderID, Guid ProductID);
    }
}
