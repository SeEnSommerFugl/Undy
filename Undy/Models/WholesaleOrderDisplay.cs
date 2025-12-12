namespace Undy.Models {
    public class WholesaleOrderDisplay {
        //Properties for WholesaleOrder
        public Guid WholesaleOrderID { get; set; }
        public int WholesaleOrderNumber { get; set; }
        public string DisplayWholesaleOrderNumber { get; set; }
        public DateOnly ExpectedDeliveryDate { get; set; }
        public DateOnly? DeliveryDate { get; set; }
        public DateOnly WholesaleOrderDate { get; set; }
        public string OrderStatus { get; set; }

        //Properties for ProductWholesaleOrder
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int QuantityReceived { get; set; }

        //Properties for Product
        public Guid ProductID { get; set; }
        public string ProductNumber { get; set; }
        public string ProductName { get; set; }
        public string Size { get; set; }
        public string Colour { get; set; }
    }
}
