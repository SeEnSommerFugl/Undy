namespace Undy.Features.Repository
{
    public class WholesaleOrderDBRepository : BaseDBRepository<WholesaleOrder, Guid>
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

        // Map data record to entity 
        protected override WholesaleOrder Map(IDataRecord r) => new WholesaleOrder
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
        };

        // Parameter binding for id
        protected override void BindId(SqlCommand cmd, Guid id)
        {
            cmd.Parameters.Add("@WholesaleOrderID", SqlDbType.UniqueIdentifier).Value = id;
        }

        // Parameter binding for insert
        protected override void BindInsert(SqlCommand cmd, WholesaleOrder e)
        {
            cmd.Parameters.Add("@WholesaleOrderID", SqlDbType.UniqueIdentifier).Value = e.WholesaleOrderID;
            cmd.Parameters.Add("@ExpectedDeliveryDate", SqlDbType.Date).Value = e.ExpectedDeliveryDate;
            cmd.Parameters.Add("@OrderDate", SqlDbType.Date).Value = e.OrderDate;
            cmd.Parameters.Add("@DeliveryDate", SqlDbType.Date).Value = e.DeliveryDate;
            cmd.Parameters.Add("@OrderStatus", SqlDbType.NVarChar, 50).Value = e.OrderStatus;

        }

        // Parameter binding for update
        protected override void BindUpdate(SqlCommand cmd, WholesaleOrder e)
        {
            cmd.Parameters.Add("@WholesaleOrderID", SqlDbType.UniqueIdentifier).Value = e.WholesaleOrderID;
            cmd.Parameters.Add("@ExpectedDeliveryDate", SqlDbType.Date).Value = e.ExpectedDeliveryDate;
            cmd.Parameters.Add("@OrderDate", SqlDbType.Date).Value = e.OrderDate;
            cmd.Parameters.Add("@DeliveryDate", SqlDbType.Date).Value = e.DeliveryDate;
            cmd.Parameters.Add("@OrderStatus", SqlDbType.NVarChar, 50).Value = e.OrderStatus;

        }

        // Get key from entity
        protected override Guid GetKey(WholesaleOrder e) => e.WholesaleOrderID;

        public async Task ConfirmFullReceiveAsync(int wholesaleOrderNumber)
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand("usp_Update_WholesaleOrder", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@WholesaleOrderNumber", SqlDbType.Int)
               .Value = wholesaleOrderNumber;

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task ConfirmPartialReceiveAsync(
            int wholesaleOrderNumber,
            string productNumber,
            int quantity)
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand("usp_UpdatePartial_WholesaleOrder", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@WholesaleOrderNumber", SqlDbType.Int)
                .Value = wholesaleOrderNumber;

            cmd.Parameters.Add("@ProductNumber", SqlDbType.NVarChar, 255)
                .Value = productNumber;

            cmd.Parameters.Add("@Quantity", SqlDbType.Int)
                .Value = quantity;

            await cmd.ExecuteNonQueryAsync();
        }
    };
}

