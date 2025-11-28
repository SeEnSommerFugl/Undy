using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels.Helpers
{
    internal class SalesOrderService
    {
        private readonly SalesOrderDBRepository _salesOrderRepo;
        //private readonly ProductSalesOrderDBRepository _productSalesOrderRepo;
        //private SqlConnection con = Db.OpenConnection();

        public SalesOrderService()
        {
            _salesOrderRepo = new SalesOrderDBRepository();
        }

        public async Task CreateSalesOrderWithProducts(SalesOrder salesOrder, List<ProductSalesOrder> productSalesOrder)
        {
            using (var transaction = con.BeginTransaction())
            {
                try
                {
                    await _salesOrderRepo.AddAsync(salesOrder);

                    foreach (var product in productSalesOrder)
                    {
                        product.SalesOrderID = salesOrder.SalesOrderID;
                        await _ProductSalesOrderDBRepository.AddAsync(product);
                    }
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
}
