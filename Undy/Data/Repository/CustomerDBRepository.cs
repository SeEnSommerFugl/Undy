using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Undy.Models;

namespace Undy.Data.Repository
{
    class CustomerDBRepository : BaseDBRepository<Customer, Guid> {
        protected override string SqlSelectAll => "SELECT * FROM vw_Customers";

        protected override Customer Map(IDataRecord r) {
            return new Customer {
                CustomerID = r.GetGuid(r.GetOrdinal("CustomerID")),
                CustomerNumber = r.GetInt32(r.GetOrdinal("CustomerNumber")),
                DisplayCustomerNumber = r.GetString(r.GetOrdinal("DisplayCustomerNumber")),
                FullName = r.GetString(r.GetOrdinal("FullName")),
                FirstName = r.GetString(r.GetOrdinal("FirstName")),
                LastName = r.GetString(r.GetOrdinal("LastName")),
                Email = r.GetString(r.GetOrdinal("Email")),
                PhoneNumber = r.GetInt32(r.GetOrdinal("PhoneNumber")),
                Address = r.GetString(r.GetOrdinal("Address")),
                City = r.GetString(r.GetOrdinal("City")),
                PostalCode = r.GetInt32(r.GetOrdinal("PostalCode"))
            };
        }

        protected override Guid GetKey(Customer e) => e.CustomerID;
    }
}
