namespace Undy.Data.Repository
{
    internal class WholesaleOrderDisplayDBRepository : BaseDBRepository<WholesaleOrderDisplay, Guid>
    {
        protected override string SqlSelectAll => "SELECT * FROM vw_WholesaleOrders";

        protected override WholesaleOrderDisplay Map(IDataRecord r) => new WholesaleOrderDisplay
        {
            WholesaleOrderID = r.GetGuid(r.GetOrdinal("WholesaleOrderID")),
            WholesaleOrderNumber = r.GetInt32(r.GetOrdinal("WholesaleOrderNumber")),
            DisplayWholesaleOrderNumber = r.GetString(r.GetOrdinal("DisplayWholesaleOrderNumber")),
            WholesaleOrderDate = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("WholesaleOrderDate"))),
            ExpectedDeliveryDate = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("ExpectedDeliveryDate"))),
            DeliveryDate = r.IsDBNull(r.GetOrdinal("DeliveryDate"))
                            ? null
            : DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("DeliveryDate"))),

            OrderStatus = r.GetString(r.GetOrdinal("OrderStatus")),
            Quantity = r.GetInt32(r.GetOrdinal("Quantity")),
            UnitPrice = r.GetDecimal(r.GetOrdinal("UnitPrice")),
            QuantityReceived = r.GetInt32(r.GetOrdinal("QuantityReceived")),
            ProductID = r.GetGuid(r.GetOrdinal("ProductID")),
            ProductNumber = r.GetString(r.GetOrdinal("ProductNumber")),
            ProductName = r.GetString(r.GetOrdinal("ProductName")),
            Size = r.GetString(r.GetOrdinal("Size")),
            Colour = r.GetString(r.GetOrdinal("Colour"))
        };

        protected override Guid GetKey(WholesaleOrderDisplay e) => e.WholesaleOrderID;
    }
}
