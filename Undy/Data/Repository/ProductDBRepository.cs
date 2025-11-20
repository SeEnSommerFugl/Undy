using System.Data;
using Microsoft.Data.SqlClient;
using Undy.Models;

namespace Undy.Data.Repository
{
    public class ProductDBRepository : BaseDBRepository<Product, Guid>
    {
        // View for selecting all
        protected override string SqlSelectAll => "vm_Product";

        // Stored procedure for getting by id
        protected override string SqlSelectById => "usp_SelectById_Product";

        // Stored procedures for adding (insert into)
        protected override string SqlInsert => "usp_Insert_Product";

        // Stored procedure for updating
        protected override string SqlUpdate => "usp_Update_Product";

        // Stored procedure for deleting  
        protected override string SqlDeleteById => "usp_DeleteById_Product";

        // Map data record to entity
        protected override Product Map(IDataRecord r) => new Product
        {
            ProductID = r.GetGuid(r.GetOrdinal("Product_ID")),
            ProductNumber = r.GetInt32(r.GetOrdinal("ProductNumber")),
            ProductName = r.GetString(r.GetOrdinal("ProductName")),
            Price = r.GetDecimal(r.GetOrdinal("Price")),
            Size = r.GetString(r.GetOrdinal("Size")),
            Colour = r.GetString(r.GetOrdinal("Colour")),
            ProductCatalogueID = r.IsDBNull(r.GetOrdinal("ProductCatalogue_ID"))
               ? (Guid?)null
               : r.GetGuid(r.GetOrdinal("ProductCatalogue_ID"))
        };

        // Parameter binding for id
        protected override void BindId(SqlCommand cmd, Guid id)
        {
            cmd.Parameters.Add("@Product_ID", SqlDbType.UniqueIdentifier).Value = id;
        }

        // Parameter binding for insert
        protected override void BindInsert(SqlCommand cmd, Product e)
        {
            cmd.Parameters.Add("@Product_ID", SqlDbType.UniqueIdentifier).Value = e.ProductID;
            cmd.Parameters.Add("@ProductNumber", SqlDbType.Int).Value = e.ProductNumber;
            cmd.Parameters.Add("@ProductName", SqlDbType.NVarChar, 255).Value = e.ProductName;
            cmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = e.Price;
            cmd.Parameters.Add("@Size", SqlDbType.NVarChar, 20).Value = e.Size;
            cmd.Parameters.Add("@Colour", SqlDbType.NVarChar, 20).Value = e.Colour;
            cmd.Parameters.Add("@ProductCatalogue_ID", SqlDbType.UniqueIdentifier)
                .Value = (object?)e.ProductCatalogueID ?? DBNull.Value;
        }
        // Parameter binding for update
        protected override void BindUpdate(SqlCommand cmd, Product e)
        {
            cmd.Parameters.Add("@Product_ID", SqlDbType.Int).Value = e.ProductID;
            cmd.Parameters.Add("@ProductNumber", SqlDbType.Int).Value = e.ProductNumber;
            cmd.Parameters.Add("@ProductName", SqlDbType.NVarChar, 255).Value = e.ProductName;
            cmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = e.Price;
            cmd.Parameters.Add("@Size", SqlDbType.NVarChar, 20).Value = e.Size;
            cmd.Parameters.Add("@Colour", SqlDbType.NVarChar, 20).Value = e.Colour;
            cmd.Parameters.Add("@ProductCatalogue_ID", SqlDbType.UniqueIdentifier)
                .Value = (object?)e.ProductCatalogueID ?? DBNull.Value;
        }

        protected override Guid GetKey(Product e) => e.ProductID;


    }
}
