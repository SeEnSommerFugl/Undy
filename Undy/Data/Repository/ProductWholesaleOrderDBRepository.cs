using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Undy.Models;

namespace Undy.Data.Repository {
    internal class ProductWholesaleOrderDBRepository : BaseDBRepository<ProductWholesaleOrder, Guid> {
        protected override Guid GetKey(ProductWholesaleOrder entity) {
            throw new NotImplementedException();
        }

        protected override ProductWholesaleOrder Map(IDataRecord r) {
            throw new NotImplementedException();
        }
    }
}
