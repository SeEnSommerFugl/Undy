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
        protected override string SqlSelectById => "usp_SelectById_SalesOrders";

        // Stored procedures for adding (insert into)
        protected override string SqlInsert => "usp_Insert_SalesOrders";

        // Stored procedure for updating
        protected override string SqlUpdate => "usp_Update_SalesOrders";

        // Stored procedure for deleting
        protected override string SqlDeleteById => "usp_DeleteById_SalesOrders";

        // Map data record to entity
        protected override SalesOrder Map(IDataRecord r) => new SalesOrder
        {
            SalesOrderID = r.GetGuid(r.GetOrdinal("SalesOrder_ID")),
            ProductNumber = r.GetInt32(r.GetOrdinal("ProductNumber")),
            Quantity = r.GetInt32(r.GetOrdinal("Quantity")),
            SalesOrderStatus = r.GetString(r.GetOrdinal("SalesOrderStatus")),
            ProductID = r.GetGuid(r.GetOrdinal("Product_ID"))

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
            cmd.Parameters.Add("@ProductNumber", SqlDbType.Int).Value = e.ProductNumber;
            cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = e.Quantity;
            cmd.Parameters.Add("@SalesOrderStatus", SqlDbType.NVarChar, 255).Value = e.SalesOrderStatus;
            cmd.Parameters.Add("@Product_ID", SqlDbType.UniqueIdentifier)
                .Value = (object?)e.ProductID ?? DBNull.Value;
        }
        // Parameter binding for update
        protected override void BindUpdate(SqlCommand cmd, SalesOrder e)
        {
            cmd.Parameters.Add("@SalesOrder_ID", SqlDbType.UniqueIdentifier).Value = e.SalesOrderID;
            cmd.Parameters.Add("@ProductNumber", SqlDbType.Int).Value = e.ProductNumber;
            cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = e.Quantity;
            cmd.Parameters.Add("@SalesOrderStatus", SqlDbType.NVarChar, 255).Value = e.SalesOrderStatus;
            cmd.Parameters.Add("@Product_ID", SqlDbType.UniqueIdentifier)
                .Value = (object?)e.ProductID ?? DBNull.Value;
        }

        protected override Guid GetKey(SalesOrder entity)
        {
            throw new NotImplementedException();
        }

       
    }
}
