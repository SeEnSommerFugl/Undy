
namespace Undy.Data.Repository
{
    /// <summary>
    /// SalesOrderLine repository (composite key in DB: SalesOrderID + ProductID).
    /// This is a query-repository (no BaseDBRepository<Guid> inheritance).
    /// </summary>
    public class SalesOrderLineDBRepository : BaseDBRepository<SalesOrderLine, Guid> 
    {

        protected override string SqlSelectAll => "SELECT * FROM vw_SalesOrderLines";

        protected override string SqlSelectById => "usp_Select_ProductSalesOrder_BySalesOrderID";

        protected override string SqlInsert => "usp_Insert_SalesOrderLine";

        protected override string SqlUpdate => "usp_Update_SalesOrderLine";

        protected override string SqlDeleteById => "usp_DeleteById_SalesOrderLine";

        // Map data record to entity
        protected override SalesOrderLine Map(IDataRecord r) => new SalesOrderLine
        {
            SalesOrderID = r.GetGuid(r.GetOrdinal("SalesOrderID")),
            SalesOrderNumber = r.GetInt32(r.GetOrdinal("SalesOrderNumber")),
            ProductID = r.GetGuid(r.GetOrdinal("ProductID")),
            ProductName = r.GetString(r.GetOrdinal("ProductName")),
            Quantity = r.GetInt32(r.GetOrdinal("Quantity")),
            UnitPrice = r.GetDecimal(r.GetOrdinal("UnitPrice"))
        };

        // Bind key for GetById/Delete
        protected override void BindId(SqlCommand cmd, Guid id)
        {
            cmd.Parameters.Add("@SalesOrderID", SqlDbType.UniqueIdentifier).Value = id;
        }

        // Bind insert params (includes composite PK columns)
        protected override void BindInsert(SqlCommand cmd, SalesOrderLine e)
        {
            cmd.Parameters.Add("@SalesOrderID", SqlDbType.UniqueIdentifier).Value = e.SalesOrderID;
            cmd.Parameters.Add("@ProductID", SqlDbType.UniqueIdentifier).Value = e.ProductID;
            cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = e.Quantity;

            var p = cmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal);
            p.Precision = 10;
            p.Scale = 2;
            p.Value = e.UnitPrice;
        }

        // Bind update params (needs both key columns too)
        protected override void BindUpdate(SqlCommand cmd, SalesOrderLine e)
        {
            cmd.Parameters.Add("@SalesOrderID", SqlDbType.UniqueIdentifier).Value = e.SalesOrderID;
            cmd.Parameters.Add("@ProductID", SqlDbType.UniqueIdentifier).Value = e.ProductID;
            cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = e.Quantity;

            var p = cmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal);
            p.Precision = 10;
            p.Scale = 2;
            p.Value = e.UnitPrice;
        }

        // Get key from entity
        protected override Guid GetKey(SalesOrderLine e) => e.SalesOrderID;

        //public ObservableCollection<SalesOrderLine> Items => throw new NotImplementedException();

        ///// <summary>
        ///// Loads all lines for a single SalesOrderID using a stored procedure.
        ///// </summary>
        //public async Task<IEnumerable<SalesOrderLine>> GetBySalesOrderIdAsync(Guid salesOrderId)
        //{
        //    var result = new List<SalesOrderLine>();

        //    using var con = await DB.OpenConnection();
        //    using var cmd = new SqlCommand("dbo.usp_Select_ProductSalesOrder_BySalesOrderID", con)
        //    {
        //        CommandType = CommandType.StoredProcedure
        //    };

        //    cmd.Parameters.Add("@SalesOrderID", SqlDbType.UniqueIdentifier).Value = salesOrderId;

        //    using var reader = await cmd.ExecuteReaderAsync();
        //    while (await reader.ReadAsync())
        //    {
        //        result.Add(Map(reader));
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// Inserts multiple sales order lines into dbo.ProductSalesOrder.
        ///// </summary>
        //public async Task AddRangeAsync(IEnumerable<SalesOrderLine> lines)
        //{
        //    using var con = await DB.OpenConnection();
        //    using var tx = con.BeginTransaction();

        //    try
        //    {
        //        foreach (var line in lines)
        //        {
        //            using var cmd = new SqlCommand(
        //                "INSERT INTO dbo.ProductSalesOrder (SalesOrderID, ProductID, Quantity, UnitPrice) " +
        //                "VALUES (@SalesOrderID, @ProductID, @Quantity, @UnitPrice)",
        //                con,
        //                tx);

        //            cmd.Parameters.Add("@SalesOrderID", SqlDbType.UniqueIdentifier).Value = line.SalesOrderID;
        //            cmd.Parameters.Add("@ProductID", SqlDbType.UniqueIdentifier).Value = line.ProductID;
        //            cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = line.Quantity;
        //            cmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal).Value = line.UnitPrice;

        //            await cmd.ExecuteNonQueryAsync();
        //        }

        //        await tx.CommitAsync();
        //    }
        //    catch
        //    {
        //        await tx.RollbackAsync();
        //        throw;
        //    }
        //}

        //private static SalesOrderLine Map(IDataRecord r)
        //{
        //    return new SalesOrderLine
        //    {
        //        SalesOrderID = r.GetGuid(r.GetOrdinal("SalesOrderID")),
        //        SalesOrderNumber = r.GetInt32(r.GetOrdinal("SalesOrderNumber")),
        //        ProductID = r.GetGuid(r.GetOrdinal("ProductID")),
        //        ProductName = r.GetString(r.GetOrdinal("ProductName")),
        //        Quantity = r.GetInt32(r.GetOrdinal("Quantity")),
        //        UnitPrice = r.GetDecimal(r.GetOrdinal("UnitPrice"))
        //        // LineTotal is computed in the model (Quantity * UnitPrice)
        //    };
        //}
    }
}
