using Microsoft.Data.SqlClient;
using Undy.Data;
using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels.Helpers
{
    public class SalesOrderService
    {
        private readonly SalesOrderDBRepository _salesOrderRepo;
        private readonly ProductSalesOrderDBRepository _productSalesOrderRepo;

        public SalesOrderService()
        {
            _salesOrderRepo = new SalesOrderDBRepository();
            _productSalesOrderRepo = new ProductSalesOrderDBRepository();
        }

        public async Task CreateSalesOrderWithProducts(SalesOrder salesOrder, List<ProductSalesOrder> productSalesOrderLines)
        {
            using var con = await DB.OpenConnection();
            using var transaction = con.BeginTransaction();

            try
            {
                //await _salesOrderRepo.AddRangeAsync(salesOrder, con, transaction);
                await _productSalesOrderRepo.AddRangeAsync(productSalesOrderLines, con, transaction);

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
