using System.Data;
using Microsoft.Data.SqlClient;
using Undy.Models;
using Undy.Features.Base;

namespace Undy.Features.SalesOrders
{
    public class SalesOrderDBRepository : BaseDBRepository<SalesOrder, Guid>
    {
        // View for selecting all
        protected override string SqlSelectAll => "SELECT * FROM vw_SalesOrders";

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
            SalesOrderID = r.GetGuid(r.GetOrdinal("SalesOrderID")),
            //CustomerID = r.GetGuid(r.GetOrdinal("CustomerID")),
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
            cmd.Parameters.Add("@OrderStatus", SqlDbType.NVarChar, 255).Value = e.OrderStatus;
            cmd.Parameters.Add("@PaymentStatus", SqlDbType.NVarChar).Value = e.PaymentStatus;
            cmd.Parameters.Add("@SalesDate", SqlDbType.Date).Value = e.SalesDate;
            cmd.Parameters.Add("@CustomerNumber", SqlDbType.NVarChar, 255).Value = e.CustomerNumber;
        }
        // Parameter binding for update
        protected override void BindUpdate(SqlCommand cmd, SalesOrder e)
        {
            cmd.Parameters.Add("@OrderStatus", SqlDbType.NVarChar, 255).Value = e.OrderStatus;
            cmd.Parameters.Add("@PaymentStatus", SqlDbType.NVarChar).Value = e.PaymentStatus;
        }

        protected override Guid GetKey(SalesOrder e) => e.SalesOrderID;
    }
}
