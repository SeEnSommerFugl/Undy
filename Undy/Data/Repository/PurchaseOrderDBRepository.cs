using System.Data;
using System.Windows.Controls.Primitives;
using Microsoft.Data.SqlClient;
using Undy.Models;

namespace Undy.Data.Repository
{
    public class PurchaseOrderDBRepository : BaseDBRepository<PurchaseOrder, Guid>
    {
        protected override string SqlSelectAll => "vw_PurchaseOrders";

        //TODO: Stored procedure for getting by id
        protected override string SqlSelectById => "usp_SelectByID_PurchaseOrders";

        //TODO: Stored procedures for adding (insert into)
        // usp_Insert_PurchaseOrders
        protected override string SqlInsert => @"
            INSERT INTO PurchaseOrder (PurchaseOrder_ID, ExpectedDeliveryDate, OrderDate, DeliveryDate, OrderStatus, Product_ID)
            VALUES (@PurchaseOrder_ID, @ExpectedDeliveryDate, @OrderDate, @DeliveryDate, @OrderStatus, @Product_ID);";

        //TODO: Stored procedure for updating
        protected override string SqlUpdate => @"
            UPDATE PurchaseOrder
                SET ExpectedDeliveryDate = @ExpectedDeliveryDate,
                OrderDate = @OrderDate,
                DeliveryDate = @DeliveryDate,
                OrderStatus = @OrderStatus,
                Product_ID = @Product_ID
            WHERE PurchaseOrder_ID = @PurchaseOrder_ID;";

        //TODO: Stored procedure for deleting
        protected override string SqlDeleteById => @"
            DELETE FROM PurchaseOrder
            WHERE PurchaseOrder_ID = @PurchaseOrder_ID";

        protected override void BindId(SqlCommand cmd, Guid id)
        {
            cmd.Parameters.Add("@PurchaseOrder_ID", SqlDbType.UniqueIdentifier).Value = id;
        }

        protected override void BindInsert(SqlCommand cmd, PurchaseOrder e)
        {
            cmd.Parameters.Add("@PurchaseOrder_ID", SqlDbType.UniqueIdentifier).Value = e.PurchaseOrderID;
            cmd.Parameters.Add("@ExpectedDeliveryDate", SqlDbType.Date).Value = e.ExpectedDeliveryDate;
            cmd.Parameters.Add("@OrderDate", SqlDbType.Date).Value = e.OrderDate;
            cmd.Parameters.Add("@DeliveryDate", SqlDbType.Date).Value = e.DeliveryDate;
            cmd.Parameters.Add("@OrderStatus", SqlDbType.NVarChar, 50).Value = e.OrderStatus;
            cmd.Parameters.Add("@Product_ID", SqlDbType.UniqueIdentifier)
                .Value = (object?)e.ProductID ?? DBNull.Value;
        }

        protected override void BindUpdate(SqlCommand cmd, PurchaseOrder e)
        {
            cmd.Parameters.Add("@PurchaseOrder_ID", SqlDbType.UniqueIdentifier).Value = e.PurchaseOrderID;
            cmd.Parameters.Add("@ExpectedDeliveryDate", SqlDbType.Date).Value = e.ExpectedDeliveryDate;
            cmd.Parameters.Add("@OrderDate", SqlDbType.Date).Value = e.OrderDate;
            cmd.Parameters.Add("@DeliveryDate", SqlDbType.Date).Value = e.DeliveryDate;
            cmd.Parameters.Add("@OrderStatus", SqlDbType.NVarChar, 50).Value = e.OrderStatus;
            cmd.Parameters.Add("@Product_ID", SqlDbType.UniqueIdentifier)
                .Value = (object?)e.ProductID ?? DBNull.Value;
        }

        protected override Guid GetKey(PurchaseOrder e) => e.PurchaseOrderID;


        protected override PurchaseOrder Map(IDataRecord r) => new PurchaseOrder
        {
            PurchaseOrderID = r.GetGuid(r.GetOrdinal("PurchaseOrder_ID")),
            ExpectedDeliveryDate = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("ExpectedDeliveryDate"))),
            OrderDate = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("OrderDate"))),
            DeliveryDate = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("DeliveryDate"))),
            OrderStatus = r.GetString(r.GetOrdinal("OrderStatus")),
            ProductID = r.GetGuid(r.GetOrdinal("Product_ID"))

        };
    }
}
