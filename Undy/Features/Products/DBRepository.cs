using System.Data;
using Microsoft.Data.SqlClient;
using Undy.Models;
using Undy.Features.Base;

namespace Undy.Features.Products
{
    public class ProductDBRepository : BaseDBRepository<Product, Guid>
    {
        // View for selecting all
        protected override string SqlSelectAll => "SELECT * FROM vw_Products";

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
            ProductID = r.GetGuid(r.GetOrdinal("ProductID")),
            ProductNumber = r.GetString(r.GetOrdinal("ProductNumber")),
            ProductName = r.GetString(r.GetOrdinal("ProductName")),
            Price = r.GetDecimal(r.GetOrdinal("Price")),
            Size = r.GetString(r.GetOrdinal("Size")),
            Colour = r.GetString(r.GetOrdinal("Colour")),
            NumberInStock = r.GetInt32(r.GetOrdinal("NumberInStock")),
        };

        // Parameter binding for id
        protected override void BindId(SqlCommand cmd, Guid id)
        {
            cmd.Parameters.Add("@Product_ID", SqlDbType.UniqueIdentifier).Value = id;
        }

        // Parameter binding for insert
        protected override void BindInsert(SqlCommand cmd, Product e)
        {
            cmd.Parameters.Add("@ProductID", SqlDbType.UniqueIdentifier).Value = e.ProductID;
            cmd.Parameters.Add("@ProductNumber", SqlDbType.NVarChar).Value = e.ProductNumber;
            cmd.Parameters.Add("@ProductName", SqlDbType.NVarChar, 255).Value = e.ProductName;
            cmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = e.Price;
            cmd.Parameters.Add("@Size", SqlDbType.NVarChar, 20).Value = e.Size;
            cmd.Parameters.Add("@Colour", SqlDbType.NVarChar, 20).Value = e.Colour;
            cmd.Parameters.Add("@NumberInStock", SqlDbType.Int).Value = e.NumberInStock;
        }
        // Parameter binding for update
        protected override void BindUpdate(SqlCommand cmd, Product e)
        {
            cmd.Parameters.Add("@ProductID", SqlDbType.UniqueIdentifier).Value = e.ProductID;
            cmd.Parameters.Add("@ProductNumber", SqlDbType.NVarChar).Value = e.ProductNumber;
            cmd.Parameters.Add("@ProductName", SqlDbType.NVarChar, 255).Value = e.ProductName;
            cmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = e.Price;
            cmd.Parameters.Add("@Size", SqlDbType.NVarChar, 20).Value = e.Size;
            cmd.Parameters.Add("@Colour", SqlDbType.NVarChar, 20).Value = e.Colour;
            cmd.Parameters.Add("@NumberInStock", SqlDbType.Int).Value = e.NumberInStock;
        }

        protected override Guid GetKey(Product e) => e.ProductID;


    }
}
