using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels.Helpers {
    internal class SalesOrderService {
        private readonly SalesOrderDBRepository _salesOrderRepo;
        //private readonly ProductSalesOrderDBRepository _productSalesOrderRepo;
        //private SqlConnection con = Db.OpenConnection();

        public SalesOrderService() {
            _salesOrderRepo = new SalesOrderDBRepository(con);
        }

        public void CreateSalesOrderWithProducts(SalesOrder salesOrder, List<ProductSalesOrder> productSalesOrder) {
            using(var transaction = con.BeginTransaction()) {
                try {
                    _salesOrderRepo.AddAsync(salesOrder);

                    foreach (var product in productSalesOrder) {
                        product.SalesOrderID = salesOrder.SalesOrderID;
                        _ProductSalesOrderDBRepository.Add(product);
                    }
                    transaction.Commit();
                } catch (Exception) {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
