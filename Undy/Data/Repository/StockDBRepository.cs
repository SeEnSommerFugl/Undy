using System.Data;
using Microsoft.Data.SqlClient;
using Undy.Models;

namespace Undy.Data.Repository
{
    public class StockDBRepository : BaseDBRepository<Stock, Guid>
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

        protected override void BindInsert(SqlCommand cmd, Stock entity)
        {
            throw new NotImplementedException();
        }

        protected override void BindUpdate(SqlCommand cmd, Stock entity)
        {
            throw new NotImplementedException();
        }

        protected override Guid GetKey(Stock entity)
        {
            throw new NotImplementedException();
        }

        protected override Stock Map(IDataRecord r)
        {
            throw new NotImplementedException();
        }
    }
}
