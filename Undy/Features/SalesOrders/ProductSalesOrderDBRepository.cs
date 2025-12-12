namespace Undy.Data.Repository
{
    public class ProductSalesOrderDBRepository : BaseDBRepository<ProductSalesOrder, Guid>
    {
        // View for selecting all
        protected override string SqlSelectAll => "SELECT * FROM vw_SalesOrders";

        // Stored procedure for getting by id
        protected override string SqlSelectById => "usp_SelectById_Product";

        // Stored procedures for adding (insert into)
        protected override string SqlInsert => "usp_Insert_ProductSalesOrder";

        // Stored procedure for updating
        protected override string SqlUpdate => "usp_Update_ProductSalesOrder";

        // Stored procedure for deleting  
        protected override string SqlDeleteById => "usp_DeleteById_Product";

        // Map data record to entity
        protected override ProductSalesOrder Map(IDataRecord r) => new ProductSalesOrder
        {
            SalesOrderID = r.GetGuid(r.GetOrdinal("SalesOrderID")),
            ProductID = r.GetGuid(r.GetOrdinal("ProductID")),
            Quantity = r.GetInt32(r.GetOrdinal("Quantity")),
            UnitPrice = r.GetDecimal(r.GetOrdinal("UnitPrice"))
        };

        // Parameter binding for id
        protected override void BindId(SqlCommand cmd, Guid id)
        {
            cmd.Parameters.Add("@SalesOrderID", SqlDbType.UniqueIdentifier).Value = id;
        }

        // Parameter binding for insert
        protected override void BindInsert(SqlCommand cmd, ProductSalesOrder e)
        {
            cmd.Parameters.Add(@"SalesOrderID", SqlDbType.UniqueIdentifier).Value = e.SalesOrderID;
            cmd.Parameters.Add("@ProductNumber", SqlDbType.NVarChar, 255).Value = e.ProductNumber;
            cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = e.Quantity;
            cmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal).Value = e.UnitPrice;

        }
        // Parameter binding for update
        protected override void BindUpdate(SqlCommand cmd, ProductSalesOrder e)
        {
            cmd.Parameters.Add("@SalesOrderID", SqlDbType.UniqueIdentifier).Value = e.SalesOrderID;
            cmd.Parameters.Add("@ProductID", SqlDbType.UniqueIdentifier).Value = e.ProductID;
            cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = e.Quantity;
            cmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal).Value = e.UnitPrice;
        }

        protected override Guid GetKey(ProductSalesOrder e) => e.SalesOrderID;
    }
}
