namespace Undy.Data.Repository
{
    public class WholesaleOrderLineDBRepository : BaseDBRepository<WholesaleOrderLine, Guid>
    {
        protected override string SqlSelectAll => "SELECT * FROM vw_WholesaleOrderLines";

        protected override string SqlSelectById => "dbo.usp_Select_ProductWholesaleOrder_ByWholesaleOrderID";

        // FIX: this proc exists in your SQL
        protected override string SqlInsert => "dbo.usp_Insert_ProductWholesaleOrder";

        // These procs do NOT exist in your SQL script.
        // Wholesale lines are not updated/deleted via generic CRUD - receiving is handled by usp_ProcessWholesaleReceipt_Line.
        protected override string SqlUpdate => "";
        protected override string SqlDeleteById => "";

        // Map data record to entity
        protected override WholesaleOrderLine Map(IDataRecord r) => new WholesaleOrderLine
        {
            WholesaleOrderID = r.GetGuid(r.GetOrdinal("WholesaleOrderID")),
            ProductID = r.GetGuid(r.GetOrdinal("ProductID")),
            ProductNumber = r.GetString(r.GetOrdinal("ProductNumber")),
            ProductName = r.GetString(r.GetOrdinal("ProductName")),
            Quantity = r.GetInt32(r.GetOrdinal("Quantity")),
            UnitPrice = r.GetDecimal(r.GetOrdinal("UnitPrice")),
            QuantityReceived = r.GetInt32(r.GetOrdinal("QuantityReceived")),
            QuantityPending = r.GetInt32(r.GetOrdinal("QuantityPending")),
        };

        // Bind ID
        protected override void BindId(SqlCommand cmd, Guid id)
        {
            cmd.Parameters.Add("@WholesaleOrderID", SqlDbType.UniqueIdentifier).Value = id;
        }

        // Bind insert params (matches dbo.usp_Insert_ProductWholesaleOrder)
        protected override void BindInsert(SqlCommand cmd, WholesaleOrderLine e)
        {
            cmd.Parameters.Add("@WholesaleOrderID", SqlDbType.UniqueIdentifier).Value = e.WholesaleOrderID;
            cmd.Parameters.Add("@ProductID", SqlDbType.UniqueIdentifier).Value = e.ProductID;
            cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = e.Quantity;

            var p = cmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal);
            p.Precision = 10;
            p.Scale = 2;
            p.Value = e.UnitPrice;

            // IMPORTANT: do NOT send @QuantityReceived here.
            // The proc sets QuantityReceived = 0 itself.
        }

        // If something calls Update through this repo, fail fast with a clear message.
        protected override void BindUpdate(SqlCommand cmd, WholesaleOrderLine e)
            => throw new NotSupportedException("WholesaleOrderLine updates are handled via dbo.usp_ProcessWholesaleReceipt_Line (receiving).");

        // Get composite key from entity (kept as-is: wholesale order id)
        protected override Guid GetKey(WholesaleOrderLine e) => e.WholesaleOrderID;

        public async Task ProcessReceiptLinesAsync(IEnumerable<(Guid WholesaleOrderID, Guid ProductID, int ReceiveQuantity)> receipts)
        {
            var list = receipts.ToList();
            if (list.Count == 0) return;

            using var con = await DB.OpenConnection();
            using var tx = (SqlTransaction)await con.BeginTransactionAsync();

            try
            {
                foreach (var r in list)
                {
                    using var cmd = new SqlCommand("dbo.usp_ProcessWholesaleReceipt_Line", con, tx);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@WholesaleOrderID", SqlDbType.UniqueIdentifier).Value = r.WholesaleOrderID;
                    cmd.Parameters.Add("@ProductID", SqlDbType.UniqueIdentifier).Value = r.ProductID;
                    cmd.Parameters.Add("@ReceiveQuantity", SqlDbType.Int).Value = r.ReceiveQuantity;

                    await cmd.ExecuteNonQueryAsync();
                }

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}
