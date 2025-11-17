using System.Data;
using Microsoft.Data.SqlClient;
using Undy.Models;

namespace Undy.Data.Repository
{
    public class SalesOrderDBRepository : BaseDBRepository<SalesOrder, Guid>
    {
        protected override string SqlSelectAll => throw new NotImplementedException();

        protected override string SqlSelectById => throw new NotImplementedException();

        protected override string SqlInsert => throw new NotImplementedException();

        protected override string SqlUpdate => throw new NotImplementedException();

        protected override string SqlDeleteById => throw new NotImplementedException();

        protected override void BindId(SqlCommand cmd, Guid id)
        {
            throw new NotImplementedException();
        }

        protected override void BindInsert(SqlCommand cmd, SalesOrder entity)
        {
            throw new NotImplementedException();
        }

        protected override void BindUpdate(SqlCommand cmd, SalesOrder entity)
        {
            throw new NotImplementedException();
        }

        protected override Guid GetKey(SalesOrder entity)
        {
            throw new NotImplementedException();
        }

        protected override SalesOrder Map(IDataRecord r)
        {
            throw new NotImplementedException();
        }
    }
}
