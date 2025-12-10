namespace Undy.Features.Repository
{
    public class ProductWholesaleOrderDBRepository : BaseDBRepository<ProductWholesaleOrder, Guid>
    {

        // View for selecting all
        protected override string SqlSelectAll => "SELECT * FROM vw_WholesaleOrders";

        // Stored procedure for getting by id
        protected override string SqlSelectById => "usp_SelectById_WholesaleOrder";

        // Stored procedures for adding (insert into)
        protected override string SqlInsert => "usp_Insert_WholesaleOrder";

        // Stored procedure for updating
        protected override string SqlUpdate => "usp_Update_WholesaleOrder";

        // Stored procedure for deleting
        protected override string SqlDeleteById => "usp_DeleteById_WholesaleOrder";

        // Stored procedure for partial orders
        protected override string SqlPartialInsert => "usp_InsertPartial_WholesaleOrder";

        protected override ProductWholesaleOrder Map(IDataRecord r) => new ProductWholesaleOrder
        {
            WholesaleOrderID = r.GetGuid(r.GetOrdinal("WholesaleOrderID")),
            ProductID = r.GetGuid(r.GetOrdinal("ProductID")),
            Quantity = r.GetInt32(r.GetOrdinal("Quantity")),
            UnitPrice = r.GetDecimal(r.GetOrdinal("UnitPrice")),
            QuantityReceived = r.GetInt32(r.GetOrdinal("QuantityReceived"))
        };

        // Parameter binding for id
        protected override void BindId(SqlCommand cmd, Guid id)
        {
            cmd.Parameters.Add("@WholesaleOrderID", SqlDbType.UniqueIdentifier).Value = id;
        }

        // Parameter binding for insert
        protected override void BindInsert(SqlCommand cmd, ProductWholesaleOrder e)
        {
            cmd.Parameters.Add("@WholesaleOrderID", SqlDbType.UniqueIdentifier).Value = e.WholesaleOrderID;
            cmd.Parameters.Add("@ProductID", SqlDbType.UniqueIdentifier).Value = e.ProductID;
            cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = e.Quantity;
            cmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal).Value = e.UnitPrice;
            cmd.Parameters.Add("@QuantityReceived", SqlDbType.Int).Value = e.QuantityReceived;
        }
        // Parameter binding for update
        protected override void BindUpdate(SqlCommand cmd, ProductWholesaleOrder e)
        {
            cmd.Parameters.Add("@WholesaleOrderID", SqlDbType.UniqueIdentifier).Value = e.WholesaleOrderID;
            cmd.Parameters.Add("@ProductID", SqlDbType.UniqueIdentifier).Value = e.ProductID;
            cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = e.Quantity;
            cmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal).Value = e.UnitPrice;
            cmd.Parameters.Add("@QuantityReceived", SqlDbType.Int).Value = e.QuantityReceived;
        }

        protected override Guid GetKey(ProductWholesaleOrder e) => e.WholesaleOrderID;


    }
}

