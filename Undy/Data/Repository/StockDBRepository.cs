using System.Data;
using Microsoft.Data.SqlClient;
using Undy.Models;

namespace Undy.Data.Repository
{
    public class StockDBRepository : BaseDBRepository<Stock, Guid>
    {
        // View for selecting all
        protected override string SqlSelectAll => "vm_Stock";

        // Stored procedure for getting by id
        protected override string SqlSelectById => "usp_SelectById_Stock";

        // Stored procedures for adding (insert into)
        protected override string SqlInsert => "usp_Insert_Stock";

        // Stored procedure for updating
        protected override string SqlUpdate => "usp_Update_Stock";

        // Stored procedure for deleting
        protected override string SqlDeleteById => "usp_DeleteById_Stock";

        // Map data record to entity
        protected override Stock Map(IDataRecord r) => new Stock
        {
            StockID = r.GetGuid(r.GetOrdinal("StockID")),
            InStock = r.GetInt32(r.GetOrdinal("InStock")),
            Status = r.GetInt32(r.GetOrdinal("Status")),
            MinimumStockLimit = r.GetInt32(r.GetOrdinal("MinimumStockLimit"))

        };

        // Parameter binding for id
        protected override void BindId(SqlCommand cmd, Guid id)
        {
            cmd.Parameters.Add("@StockID", SqlDbType.UniqueIdentifier).Value = id;
        }

        // Parameter binding for insert
        protected override void BindInsert(SqlCommand cmd, Stock e)
        {
            cmd.Parameters.Add("@StockID", SqlDbType.UniqueIdentifier).Value = e.StockID;
            cmd.Parameters.Add("@InStock", SqlDbType.Int).Value = e.InStock;
            cmd.Parameters.Add("@Status", SqlDbType.Int).Value = e.Status;
            cmd.Parameters.Add("@MinimumStockLimit", SqlDbType.Int).Value = e.MinimumStockLimit;
        }

        // Parameter binding for update
        protected override void BindUpdate(SqlCommand cmd, Stock e)
        {
            cmd.Parameters.Add("@StockID", SqlDbType.UniqueIdentifier).Value = e.StockID;
            cmd.Parameters.Add("@InStock", SqlDbType.Int).Value = e.InStock;
            cmd.Parameters.Add("@Status", SqlDbType.Int).Value = e.Status;
            cmd.Parameters.Add("@MinimumStockLimit", SqlDbType.Int).Value = e.MinimumStockLimit;

        }

        // Get key from entity
        protected override Guid GetKey(Stock e) => e.StockID;

    };

}

