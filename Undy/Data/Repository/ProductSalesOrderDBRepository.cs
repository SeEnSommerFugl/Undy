using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Undy.Models;

namespace Undy.Data.Repository {
    internal class ProductSalesOrderDBRepository : BaseDBRepository<ProductSalesOrder, Guid> {
        protected override Guid GetKey(ProductSalesOrder entity) {
            throw new NotImplementedException();
        }

        protected override ProductSalesOrder Map(IDataRecord r) {
            throw new NotImplementedException();
        }
    }
}
