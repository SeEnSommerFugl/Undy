using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Undy.Data.Repository;
using Undy.Models;

namespace Undy.Features.Products
{
    public sealed class ProductDBRepository : BaseDBRepository<Product, Guid>
    {
        protected override string SqlSelectAll => "SELECT * FROM vw_Products";
        protected override string SqlInsert => "usp_Insert_Product";
        protected override string SqlUpdate => "usp_Update_Product";

        protected override Guid GetKey(Product e) => e.ProductID;

        protected override Product Map(IDataRecord r) => new Product
        {
            ProductID = r.GetGuid(r.GetOrdinal("ProductID")),
            ProductNumber = r.GetString(r.GetOrdinal("ProductNumber")),
            ProductName = r.GetString(r.GetOrdinal("ProductName")),
            Price = r.GetDecimal(r.GetOrdinal("Price")),
            Size = r.IsDBNull(r.GetOrdinal("Size")) ? "" : r.GetString(r.GetOrdinal("Size")),
            Colour = r.IsDBNull(r.GetOrdinal("Colour")) ? "" : r.GetString(r.GetOrdinal("Colour")),
            NumberInStock = r.GetInt32(r.GetOrdinal("NumberInStock"))
        };

        protected override void BindInsert(SqlCommand cmd, Product e)
        {
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@ProductID", SqlDbType.UniqueIdentifier).Value = e.ProductID;
            cmd.Parameters.Add("@ProductNumber", SqlDbType.NVarChar, 255).Value = e.ProductNumber;
            cmd.Parameters.Add("@ProductName", SqlDbType.NVarChar, 255).Value = e.ProductName;

            var pPrice = cmd.Parameters.Add("@Price", SqlDbType.Decimal);
            pPrice.Precision = 10;
            pPrice.Scale = 2;
            pPrice.Value = e.Price;

            cmd.Parameters.Add("@Size", SqlDbType.NVarChar, 255).Value = e.Size;
            cmd.Parameters.Add("@Colour", SqlDbType.NVarChar, 255).Value = e.Colour;
            cmd.Parameters.Add("@NumberInStock", SqlDbType.Int).Value = e.NumberInStock;
        }

        protected override void BindUpdate(SqlCommand cmd, Product e)
        {
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@ProductID", SqlDbType.UniqueIdentifier).Value = e.ProductID;
            cmd.Parameters.Add("@ProductNumber", SqlDbType.NVarChar, 255).Value = e.ProductNumber;
            cmd.Parameters.Add("@ProductName", SqlDbType.NVarChar, 255).Value = e.ProductName;

            var pPrice = cmd.Parameters.Add("@Price", SqlDbType.Decimal);
            pPrice.Precision = 10;
            pPrice.Scale = 2;
            pPrice.Value = e.Price;

            cmd.Parameters.Add("@Size", SqlDbType.NVarChar, 255).Value = e.Size;
            cmd.Parameters.Add("@Colour", SqlDbType.NVarChar, 255).Value = e.Colour;
            cmd.Parameters.Add("@NumberInStock", SqlDbType.Int).Value = e.NumberInStock;
        }
    }
}
