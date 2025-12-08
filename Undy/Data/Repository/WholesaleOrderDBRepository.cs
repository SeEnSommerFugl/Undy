using System.Data;
using Microsoft.Data.SqlClient;
using Undy.Models;

namespace Undy.Data.Repository
{
    public class WholesaleOrderDBRepository : BaseDBRepository<WholesaleOrder, Guid>
    {
        // View for selecting all
        protected override string SqlSelectAll => "SELECT * FROM vw_PurchaseOrders";

        // Stored procedure for getting by id
        protected override string SqlSelectById => "usp_SelectById_PurchaseOrder";

        // Stored procedures for adding (insert into)
        protected override string SqlInsert => "usp_Insert_PurchaseOrder";

        // Stored procedure for updating
        protected override string SqlUpdate => "usp_Update_PurchaseOrder";

        // Stored procedure for deleting
        protected override string SqlDeleteById => "usp_DeleteById_PurchaseOrder";

        // Stored procedure for partial orders
        protected override string SqlPartialInsert => "usp_InsertPartial_PurchaseOrder";

        // Map data record to entity 
        protected override WholesaleOrder Map(IDataRecord r) => new WholesaleOrder
        {
            PurchaseOrderID = r.GetGuid(r.GetOrdinal("PurchaseOrderID")),
            PurchaseOrderNumber = r.GetInt32(r.GetOrdinal("PurchaseOrderNumber")),
            DisplayPurchaseOrderNumber = r.GetString(r.GetOrdinal("DisplayPurchaseOrderNumber")),
            PurchaseOrderDate = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("PurchaseOrderDate"))),
            ExpectedDeliveryDate = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("ExpectedDeliveryDate"))),
            DeliveryDate = r.IsDBNull(r.GetOrdinal("DeliveryDate"))
                            ? null
            : DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("DeliveryDate"))),

            OrderStatus = r.GetString(r.GetOrdinal("OrderStatus")),
        };

        // Parameter binding for id
        protected override void BindId(SqlCommand cmd, Guid id)
        {
            cmd.Parameters.Add("@PurchaseOrder_ID", SqlDbType.UniqueIdentifier).Value = id;
        }

        // Parameter binding for insert
        protected override void BindInsert(SqlCommand cmd, WholesaleOrder e)
        {
            cmd.Parameters.Add("@PurchaseOrder_ID", SqlDbType.UniqueIdentifier).Value = e.PurchaseOrderID;
            cmd.Parameters.Add("@ExpectedDeliveryDate", SqlDbType.Date).Value = e.ExpectedDeliveryDate;
            cmd.Parameters.Add("@OrderDate", SqlDbType.Date).Value = e.OrderDate;
            cmd.Parameters.Add("@DeliveryDate", SqlDbType.Date).Value = e.DeliveryDate;
            cmd.Parameters.Add("@OrderStatus", SqlDbType.NVarChar, 50).Value = e.OrderStatus;
            
        }

        // Parameter binding for update
        protected override void BindUpdate(SqlCommand cmd, WholesaleOrder e)
        {
            cmd.Parameters.Add("@PurchaseOrder_ID", SqlDbType.UniqueIdentifier).Value = e.PurchaseOrderID;
            cmd.Parameters.Add("@ExpectedDeliveryDate", SqlDbType.Date).Value = e.ExpectedDeliveryDate;
            cmd.Parameters.Add("@OrderDate", SqlDbType.Date).Value = e.OrderDate;
            cmd.Parameters.Add("@DeliveryDate", SqlDbType.Date).Value = e.DeliveryDate;
            cmd.Parameters.Add("@OrderStatus", SqlDbType.NVarChar, 50).Value = e.OrderStatus;
            
        }

        // Get key from entity
        protected override Guid GetKey(WholesaleOrder e) => e.PurchaseOrderID;

        public async Task ConfirmFullReceiveAsync(int purchaseOrderNumber)
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand("usp_Update_PurchaseOrder", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@PurchaseOrderNumber", SqlDbType.Int)
               .Value = purchaseOrderNumber;

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task ConfirmPartialReceiveAsync(
            int purchaseOrderNumber,
            string productNumber,
            int quantity)
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand("usp_UpdatePartial_PurchaseOrder", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@PurchaseOrderNumber", SqlDbType.Int)
                .Value = purchaseOrderNumber;

            cmd.Parameters.Add("@ProductNumber", SqlDbType.NVarChar, 255)
                .Value = productNumber;

            cmd.Parameters.Add("@Quantity", SqlDbType.Int)
                .Value = quantity;

            await cmd.ExecuteNonQueryAsync();
        }
    };
}

