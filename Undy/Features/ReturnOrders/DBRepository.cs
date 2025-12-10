using System.Data;
using Microsoft.Data.SqlClient;
using Undy.Models;
using Undy.Features.Base;

namespace Undy.Features.ReturnOrders
{
    public class ReturnOrderDBRepository : BaseDBRepository<ReturnOrder, Guid>
    {
        // View for selecting all
        protected override string SqlSelectAll => "SELECT * FROM vw_ReturnOrder";

        // Stored procedure for getting by id
        protected override string SqlSelectById => "usp_SelectById_ReturnOrder";

        // Stored procedures for adding (insert into)
        protected override string SqlInsert => "usp_Insert_ReturnOrder";

        // Stored procedure for updating
        protected override string SqlUpdate => "usp_Update_ReturnOrder";

        // Stored procedure for deleting
        protected override string SqlDeleteById => "usp_DeleteById_ReturnOrder";

        // Map data record to entity
        protected override ReturnOrder Map(IDataRecord r) => new ReturnOrder
        {
            ReturnOrderID = r.GetGuid(r.GetOrdinal("ReturnOrder_ID")),
            ReturnOrderDate = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("ReturnOrderDate"))),
            SalesOrderID = r.GetGuid(r.GetOrdinal("SalesOrder_ID"))
            //missing db.null check for ProductID?
        };

        // Parameter binding for id
        protected override void BindId(SqlCommand cmd, Guid id)
        {
            cmd.Parameters.Add("@ReturnOrder_ID", SqlDbType.UniqueIdentifier).Value = id;
        }

        // Parameter binding for insert
        protected override void BindInsert(SqlCommand cmd, ReturnOrder e)
        {
            cmd.Parameters.Add("@ReturnOrder_ID", SqlDbType.UniqueIdentifier).Value = e.ReturnOrderID;
            cmd.Parameters.Add("@ReturnDate", SqlDbType.Date).Value = e.ReturnOrderDate;
            cmd.Parameters.Add("@SalesOrder_ID", SqlDbType.UniqueIdentifier).Value = e.SalesOrderID;
        }

        //  Parameter binding for update
        protected override void BindUpdate(SqlCommand cmd, ReturnOrder e)
        {
            cmd.Parameters.Add("@ReturnOrder_ID", SqlDbType.UniqueIdentifier).Value = e.ReturnOrderID;
            cmd.Parameters.Add("@ReturnDate", SqlDbType.Date).Value = e.ReturnOrderDate;
            cmd.Parameters.Add("@SalesOrder_ID", SqlDbType.UniqueIdentifier).Value = e.SalesOrderID;
        }

        // Get key from entity
        protected override Guid GetKey(ReturnOrder e) => e.ReturnOrderID;
    };
}
