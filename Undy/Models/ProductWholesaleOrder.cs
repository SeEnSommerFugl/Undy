namespace Undy.Models
{
    public class ProductWholesaleOrder
    {
        public Guid WholesaleOrderID { get; set; }
        public Guid ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int QuantityReceived { get; set; }
    }
}
