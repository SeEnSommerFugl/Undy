namespace Undy.Data.Repository
{
    public class CustomerSalesOrderDBRepository : BaseDBRepository<CustomerSalesOrder, Guid>
    {
        protected override string SqlSelectAll => "SELECT * FROM vw_CustomerSalesOrders";

        protected override CustomerSalesOrder Map(IDataRecord r) => new CustomerSalesOrder {
            SalesOrderID = r.GetGuid(r.GetOrdinal("SalesOrderID")),
            SalesOrderNumber = r.GetInt32(r.GetOrdinal("SalesOrderNumber")),
            DisplaySalesOrderNumber = r.GetString(r.GetOrdinal("DisplaySalesOrderNumber")),
            OrderStatus = r.GetString(r.GetOrdinal("OrderStatus")),
            PaymentStatus = r.GetString(r.GetOrdinal("PaymentStatus")),
            SalesDate = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("SalesDate"))),
            TotalPrice = r.GetDecimal(r.GetOrdinal("TotalPrice")),

            CustomerID = r.GetGuid(r.GetOrdinal("CustomerID")),
            CustomerNumber = r.GetInt32(r.GetOrdinal("CustomerNumber")),
            DisplayCustomerNumber = r.GetString(r.GetOrdinal("DisplayCustomerNumber")),
            FullName = r.GetString(r.GetOrdinal("FullName"))
        };

        protected override Guid GetKey(CustomerSalesOrder e) => e.SalesOrderID;
    }
}
