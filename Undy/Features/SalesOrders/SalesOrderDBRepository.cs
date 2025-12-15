namespace Undy.Data.Repository
{
    public class SalesOrderDBRepository : BaseDBRepository<SalesOrder, Guid>
    {

        // OLD: protected override string SqlSelectAll => "SELECT * FROM vw_SalesOrder";
        // Issue: SalesOrders will give 1 row per orderline, returning SalesOrderID/SalesOrderNumber/CustomerID repeatedly.
        // ListView will show same order many times, UI can't differentiate properly between "What's an order" and "What's an order line".

        // View for selecting all
        protected override string SqlSelectAll => "SELECT * FROM vw_SalesOrderHeaders";

        // Stored procedure for getting by id
        protected override string SqlSelectById => "usp_SelectById_SalesOrder";

        // Stored procedures for adding (insert into)
        protected override string SqlInsert => "usp_Insert_SalesOrder";

        // Stored procedure for updating
        protected override string SqlUpdate => "usp_Update_SalesOrder";

        // Stored procedure for deleting
        protected override string SqlDeleteById => "usp_DeleteById_SalesOrder";

        // Map data record to entity
        protected override SalesOrder Map(IDataRecord r) => new SalesOrder
        {
            // Very important: None of these fields are ever permitted to be null during runtime.
            SalesOrderID = r.GetGuid(r.GetOrdinal("SalesOrderID")),
            CustomerName = r.GetString(r.GetOrdinal("CustomerName")),
            City = r.GetString(r.GetOrdinal("City")),
            CustomerID = r.GetGuid(r.GetOrdinal("CustomerID")),
            SalesOrderNumber = r.GetInt32(r.GetOrdinal("SalesOrderNumber")),
            OrderStatus = r.GetString(r.GetOrdinal("OrderStatus")),
            PaymentStatus = r.GetString(r.GetOrdinal("PaymentStatus")),
            SalesDate = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("SalesDate"))),
            TotalPrice = r.GetDecimal(r.GetOrdinal("TotalPrice"))
        };

        // Parameter binding for id
        protected override void BindId(SqlCommand cmd, Guid id)
        {
            cmd.Parameters.Add("@SalesOrderID", SqlDbType.UniqueIdentifier).Value = id;
        }

        // Parameter binding for insert
        protected override void BindInsert(SqlCommand cmd, SalesOrder e)
        {
            cmd.Parameters.Add("@SalesOrderID", SqlDbType.UniqueIdentifier).Value = e.SalesOrderID;
            cmd.Parameters.Add("@OrderStatus", SqlDbType.NVarChar, 255).Value = e.OrderStatus;
            cmd.Parameters.Add("@SalesDate", SqlDbType.Date).Value = e.SalesDate;
            cmd.Parameters.Add("@PaymentStatus", SqlDbType.NVarChar).Value = e.PaymentStatus;
            cmd.Parameters.Add("@CustomerID", SqlDbType.UniqueIdentifier).Value = e.CustomerID;
        }

        // Parameter binding for update
        protected override void BindUpdate(SqlCommand cmd, SalesOrder e)
        {
            cmd.Parameters.Add("@SalesOrderID", SqlDbType.UniqueIdentifier).Value = e.SalesOrderID;
            cmd.Parameters.Add("@OrderStatus", SqlDbType.NVarChar, 255).Value = e.OrderStatus;
            cmd.Parameters.Add("@PaymentStatus", SqlDbType.NVarChar).Value = e.PaymentStatus;
        }

        public async Task<(Guid? SalesOrderId, string? CustomerEmail)> GetValidationInfoBySalesOrderNumberAsync(int salesOrderNumber)
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand("usp_SelectSalesOrderValidationInfo_BySalesOrderNumber", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@SalesOrderNumber", SqlDbType.Int).Value = salesOrderNumber;

            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return (null, null);

            var salesOrderId = reader.GetGuid(reader.GetOrdinal("SalesOrderID"));
            var email = reader.GetString(reader.GetOrdinal("Email"));

            return (salesOrderId, email);
        }

        protected override Guid GetKey(SalesOrder e) => e.SalesOrderID;
    }
}
