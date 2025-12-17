namespace Undy.Data.Repository
{
    public class WholesaleOrderLineDBRepository : BaseDBRepository<WholesaleOrderLine, Guid>
    {
        protected override string SqlSelectAll => "SELECT * FROM vw_WholesaleOrderLines";

        protected override string SqlSelectById => "usp_Select_ProductWholesaleOrder_ByWholesaleOrderID";

        protected override string SqlInsert => "usp_Insert_WholesaleOrderLine";

        protected override string SqlUpdate => "usp_Update_WholesaleOrderLine";

        protected override string SqlDeleteById => "usp_DeleteById_WholesaleOrderLine";

        // Map data record to entity
        protected override WholesaleOrderLine Map(IDataRecord r) => new WholesaleOrderLine
        {
            WholesaleOrderID = r.GetGuid(r.GetOrdinal("WholesaleOrderID")),
            ProductID = r.GetGuid(r.GetOrdinal("ProductID")),
            Quantity = r.GetInt32(r.GetOrdinal("Quantity")),
            UnitPrice = r.GetDecimal(r.GetOrdinal("UnitPrice")),
            QuantityReceived = r.GetInt32(r.GetOrdinal("QuantityReceived"))
        };

        // Bind ID
        protected override void BindId(SqlCommand cmd, Guid id)
        {
            cmd.Parameters.Add("@WholesaleOrderID", SqlDbType.UniqueIdentifier).Value = id;
        }

        // Bind insert params 
        protected override void BindInsert(SqlCommand cmd, WholesaleOrderLine e)
        {
            cmd.Parameters.Add("@WholesaleOrderID", SqlDbType.UniqueIdentifier).Value = e.WholesaleOrderID;
            cmd.Parameters.Add("@ProductID", SqlDbType.UniqueIdentifier).Value = e.ProductID;
            cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = e.Quantity;

            var p = cmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal);
            p.Precision = 10;
            p.Scale = 2;
            p.Value = e.UnitPrice;

            cmd.Parameters.Add("@QuantityReceived", SqlDbType.Int).Value = e.QuantityReceived;
        }

        // Bind update params (needs both key columns too)
        protected override void BindUpdate(SqlCommand cmd, WholesaleOrderLine e)
        {
            cmd.Parameters.Add("@WholesaleOrderID", SqlDbType.UniqueIdentifier).Value = e.WholesaleOrderID;
            cmd.Parameters.Add("@ProductID", SqlDbType.UniqueIdentifier).Value = e.ProductID;
            cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = e.Quantity;

            var p = cmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal);
            p.Precision = 10;
            p.Scale = 2;
            p.Value = e.UnitPrice;

            cmd.Parameters.Add("@QuantityReceived", SqlDbType.Int).Value = e.QuantityReceived;
        }

        // Get composite key from entity
        protected override Guid GetKey(WholesaleOrderLine e) => (e.WholesaleOrderID);
    }
}
