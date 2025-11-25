using System.Data;
using Microsoft.Data.SqlClient;
using Undy.Models;

namespace Undy.Data.Repository
{
    public class SalesOrderDBRepository : BaseDBRepository<SalesOrder, Guid>
    {
        // View for selecting all
        protected override string SqlSelectAll => "vw_SalesOrders";

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
            SalesOrderID = r.GetGuid(r.GetOrdinal("SalesOrder_ID")),
            OrderNumber = r.GetInt32(r.GetOrdinal("OrderNumber")),
            OrderStatus = r.GetString(r.GetOrdinal("OrderStatus")),
            PaymentStatus = r.GetString(r.GetOrdinal("PaymentStatus")),
            SalesDate = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("SalesDate"))),
            TotalPrice = r.GetDecimal(r.GetOrdinal("TotalPrice"))

            //missing db.Null check for ProductsID?
        };

        // Parameter binding for id
        protected override void BindId(SqlCommand cmd, Guid id)
        {
            cmd.Parameters.Add("@SalesOrder_ID", SqlDbType.UniqueIdentifier).Value = id;
        }

        // Parameter binding for insert
        protected override void BindInsert(SqlCommand cmd, SalesOrder e)
        {
            cmd.Parameters.Add("@SalesOrder_ID", SqlDbType.UniqueIdentifier).Value = e.SalesOrderID;
            cmd.Parameters.Add("@OrderNumber", SqlDbType.Int).Value = e.OrderNumber;
            cmd.Parameters.Add("@OrderStatus", SqlDbType.NVarChar, 255).Value = e.OrderStatus;
            cmd.Parameters.Add("@PaymentStatus", SqlDbType.NVarChar).Value = e.PaymentStatus;
            cmd.Parameters.Add("@SalesDate", SqlDbType.Date).Value = e.SalesDate;
            cmd.Parameters.Add("@TotalPrice", SqlDbType.Decimal).Value = e.TotalPrice;
        }
        // Parameter binding for update
        protected override void BindUpdate(SqlCommand cmd, SalesOrder e)
        {
            cmd.Parameters.Add("@SalesOrder_ID", SqlDbType.UniqueIdentifier).Value = e.SalesOrderID;
            cmd.Parameters.Add("@OrderNumber", SqlDbType.Int).Value = e.OrderNumber;
            cmd.Parameters.Add("@OrderStatus", SqlDbType.NVarChar, 255).Value = e.OrderStatus;
            cmd.Parameters.Add("@PaymentStatus", SqlDbType.NVarChar).Value = e.PaymentStatus;
            cmd.Parameters.Add("@SalesDate", SqlDbType.Date).Value = e.SalesDate;
            cmd.Parameters.Add("@TotalPrice", SqlDbType.Decimal).Value = e.TotalPrice;
        }

        protected override Guid GetKey(SalesOrder entity)
        {
            throw new NotImplementedException();
        }

       
    }
}
