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
        //private SqlConnection con = DB.OpenConnection();

        public SalesOrderService()
        {
            _salesOrderRepo = new SalesOrderDBRepository();
            _productSalesOrderRepo = new ProductSalesOrderDBRepository();
        }

        /*public async Task CreateSalesOrderWithProducts(SalesOrder salesOrder, List<ProductSalesOrder> productSalesOrder)
        {
            using (var transaction = con.BeginTransaction())
            {
                try
                {
                    await _salesOrderRepo.AddAsync(salesOrder);

                    foreach (var product in productSalesOrder)
                    {
                        product.SalesOrderID = salesOrder.SalesOrderID;
                        await _productSalesOrderRepo.AddAsync(product);
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }*/
        }
    }

