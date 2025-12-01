using System.Data;
using System.Windows.Controls.Primitives;
using Microsoft.Data.SqlClient;
using Undy.Models;

namespace Undy.Data.Repository
{
    public class TestWholesaleOrderDBRepository : BaseDBRepository<TestWholesaleOrder, Guid>
    {
        // View for selecting all
        protected override string SqlSelectAll => "vw_PurchaseOrder";

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
        protected override TestWholesaleOrder Map(IDataRecord r) => new TestWholesaleOrder
        {
            PurchaseOrderID = r.GetGuid(r.GetOrdinal("PurchaseOrder_ID")),
            ExpectedDeliveryDate = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("ExpectedDeliveryDate"))),
            OrderDate = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("OrderDate"))),
            DeliveryDate = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("DeliveryDate"))),
            OrderStatus = r.GetString(r.GetOrdinal("OrderStatus")),
            ProductID = r.GetGuid(r.GetOrdinal("Product_ID"))

            //missing db.null check for ProductID?
        };

        // Parameter binding for id
        protected override void BindId(SqlCommand cmd, Guid id)
        {
            cmd.Parameters.Add("@PurchaseOrder_ID", SqlDbType.UniqueIdentifier).Value = id;
        }

        // Parameter binding for insert
        protected override void BindInsert(SqlCommand cmd, TestWholesaleOrder e)
        {
            cmd.Parameters.Add("@PurchaseOrder_ID", SqlDbType.UniqueIdentifier).Value = e.PurchaseOrderID;
            cmd.Parameters.Add("@ExpectedDeliveryDate", SqlDbType.Date).Value = e.ExpectedDeliveryDate;
            cmd.Parameters.Add("@OrderDate", SqlDbType.Date).Value = e.OrderDate;
            cmd.Parameters.Add("@DeliveryDate", SqlDbType.Date).Value = e.DeliveryDate;
            cmd.Parameters.Add("@OrderStatus", SqlDbType.NVarChar, 50).Value = e.OrderStatus;
            cmd.Parameters.Add("@Product_ID", SqlDbType.UniqueIdentifier)
                .Value = (object?)e.ProductID ?? DBNull.Value;
        }

        // Parameter binding for update
        protected override void BindUpdate(SqlCommand cmd, TestWholesaleOrder e)
        {
            cmd.Parameters.Add("@PurchaseOrder_ID", SqlDbType.UniqueIdentifier).Value = e.PurchaseOrderID;
            cmd.Parameters.Add("@ExpectedDeliveryDate", SqlDbType.Date).Value = e.ExpectedDeliveryDate;
            cmd.Parameters.Add("@OrderDate", SqlDbType.Date).Value = e.OrderDate;
            cmd.Parameters.Add("@DeliveryDate", SqlDbType.Date).Value = e.DeliveryDate;
            cmd.Parameters.Add("@OrderStatus", SqlDbType.NVarChar, 50).Value = e.OrderStatus;
            cmd.Parameters.Add("@Product_ID", SqlDbType.UniqueIdentifier)
                .Value = (object?)e.ProductID ?? DBNull.Value;
        }

        // Get key from entity
        protected override Guid GetKey(TestWholesaleOrder e) => e.PurchaseOrderID;


    };
}
    
