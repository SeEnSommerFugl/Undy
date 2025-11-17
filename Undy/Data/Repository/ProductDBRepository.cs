using System.Data;
using Microsoft.Data.SqlClient;
using Undy.Models;

namespace Undy.Data.Repository
{
    public class ProductDBRepository : BaseDBRepository<Product, Guid>
    {
        protected override string SqlSelectAll => string.Empty;

        protected override string SqlSelectById => string.Empty;

        protected override string SqlInsert => string.Empty;

        protected override string SqlUpdate => string.Empty;

        protected override string SqlDeleteById => string.Empty;

        protected override void BindId(SqlCommand cmd, Guid id)
        {
            throw new NotImplementedException();
        }

        protected override void BindInsert(SqlCommand cmd, Product entity)
        {
            throw new NotImplementedException();
        }

        protected override void BindUpdate(SqlCommand cmd, Product entity)
        {
            throw new NotImplementedException();
        }

        protected override Guid GetKey(Product entity)
        {
            throw new NotImplementedException();
        }

        protected override Product Map(IDataRecord r)
        {
            throw new NotImplementedException();
        }
    }
}
