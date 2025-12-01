using System.Data;
using Undy.Models;

namespace Undy.Data.Repository
{
    public class SalesOrderDisplayDBRepository : BaseDBRepository<SalesOrderDisplay, Guid>
    {
        protected override string SqlSelectAll => "Select * From vw_SalesOrders";

        protected override SalesOrderDisplay Map(IDataRecord r)
        {
            return new SalesOrderDisplay
            {
                SalesOrderID = r.GetGuid(r.GetOrdinal("SalesOrderID")),
                OrderNumber = r.GetInt32(r.GetOrdinal("OrderNumber")),
                OrderStatus = r.GetString(r.GetOrdinal("OrderStatus")),
                PaymentStatus = r.GetString(r.GetOrdinal("PaymentStatus")),
                SalesDate = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("SalesDate"))),
                TotalPrice = r.GetDecimal(r.GetOrdinal("TotalPrice")),
                ProductName = r.GetString(r.GetOrdinal("ProductName")),
                Quantity = r.GetInt32(r.GetOrdinal("Quantity"))
            };
        }

        // Get key from entity
        protected override Guid GetKey(SalesOrderDisplay e) => e.SalesOrderID;


    };
}

