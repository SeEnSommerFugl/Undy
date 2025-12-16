using System.Data;
using Microsoft.Data.SqlClient;
using Undy.Models;

namespace Undy.Data.Repository
{
    /// <summary>
    /// Wholesale order line repository (composite key in DB: WholesaleOrderID + ProductID).
    /// Kept as a small query/command repository (no BaseDBRepository<Guid> inheritance).
    /// </summary>
    public class WholesaleOrderLineDBRepository
    {
        public async Task<IEnumerable<WholesaleOrderLine>> GetByWholesaleOrderIdAsync(Guid wholesaleOrderId)
        {
            var result = new List<WholesaleOrderLine>();

            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand(
                "SELECT WholesaleOrderID, ProductID, Quantity, UnitPrice, QuantityReceived " +
                "FROM dbo.ProductWholesaleOrder WHERE WholesaleOrderID = @WholesaleOrderID",
                con);

            cmd.Parameters.Add("@WholesaleOrderID", SqlDbType.UniqueIdentifier).Value = wholesaleOrderId;

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(Map(reader));
            }

            return result;
        }

        public async Task AddRangeAsync(IEnumerable<WholesaleOrderLine> lines)
        {
            using var con = await DB.OpenConnection();
            using var tx = con.BeginTransaction();

            try
            {
                foreach (var line in lines)
                {
                    using var cmd = new SqlCommand(
                        "INSERT INTO dbo.ProductWholesaleOrder (WholesaleOrderID, ProductID, Quantity, UnitPrice, QuantityReceived) " +
                        "VALUES (@WholesaleOrderID, @ProductID, @Quantity, @UnitPrice, @QuantityReceived)",
                        con,
                        tx);

                    cmd.Parameters.Add("@WholesaleOrderID", SqlDbType.UniqueIdentifier).Value = line.WholesaleOrderID;
                    cmd.Parameters.Add("@ProductID", SqlDbType.UniqueIdentifier).Value = line.ProductID;
                    cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = line.Quantity;
                    cmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal).Value = line.UnitPrice;
                    cmd.Parameters.Add("@QuantityReceived", SqlDbType.Int).Value = line.QuantityReceived;

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

        private static WholesaleOrderLine Map(IDataRecord r) => new WholesaleOrderLine
        {
            WholesaleOrderID = r.GetGuid(r.GetOrdinal("WholesaleOrderID")),
            ProductID = r.GetGuid(r.GetOrdinal("ProductID")),
            Quantity = r.GetInt32(r.GetOrdinal("Quantity")),
            UnitPrice = r.GetDecimal(r.GetOrdinal("UnitPrice")),
            QuantityReceived = r.GetInt32(r.GetOrdinal("QuantityReceived"))
        };
    }
}
