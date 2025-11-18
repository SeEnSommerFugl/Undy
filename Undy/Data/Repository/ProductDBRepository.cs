using System.Data;
using Microsoft.Data.SqlClient;
using Undy.Models;

namespace Undy.Data.Repository
{
    public class ProductDBRepository : BaseDBRepository<Product, Guid>
    {
        protected override string SqlSelectAll => @"
            SELECT Product_ID, ProductNumber, ProductName, Price, Size, Colour, ProductCatalogue_ID
            FROM Product"; // FROM dbo.ProductView

        protected override string SqlSelectById => @"
            SELECT Product_ID, ProductNumber, ProductName, Price, Size, Colour, ProductCatalogue_ID
            FROM Product
            WHERE Product_ID = @Product_ID";

        protected override string SqlInsert => @"
            INSERT INTO Product (Product_ID, ProductNumber, ProductName, Price, Size, Colour, ProductCatalogue_ID)
            VALUES (@Product_ID, @ProductNumber, @ProductName, @Price, @Size, @Colour, @ProductCatalogue_ID);";

        protected override string SqlUpdate => @"
            UPDATE Product
                SET ProductNumber = @ProductNumber,
                ProductName = @ProductName,
                Price = @Price,
                Size = @Size,
                Colour = @Colour,
                ProductCatalogue_ID = @ProductCatalogue_ID
            WHERE Product_ID = @Product_ID;";

        protected override string SqlDeleteById => @"
            DELETE FROM Product
            WHERE Product_ID = @Product_ID";


        protected override void BindId(SqlCommand cmd, Guid id)
        {
            cmd.Parameters.Add("@Product_ID", SqlDbType.UniqueIdentifier).Value = id;
        }

        protected override void BindInsert(SqlCommand cmd, Product e)
        {
            cmd.Parameters.Add("@Product_ID", SqlDbType.UniqueIdentifier).Value = e.ProductID;
            cmd.Parameters.Add("@ProductNumber", SqlDbType.UniqueIdentifier).Value = e.ProductNumber;
            cmd.Parameters.Add("@ProductName", SqlDbType.UniqueIdentifier).Value = e.ProductName;
            cmd.Parameters.Add("@Price", SqlDbType.UniqueIdentifier).Value = e.Price;
            cmd.Parameters.Add("@Size", SqlDbType.UniqueIdentifier).Value = e.Size;
            cmd.Parameters.Add("@Colour", SqlDbType.UniqueIdentifier).Value = e.Colour;
            cmd.Parameters.Add("ProductCatalogue_ID", SqlDbType.UniqueIdentifier)
                .Value = (object?)e.ProductCatalogueID ?? DBNull.Value;
        }

        protected override void BindUpdate(SqlCommand cmd, Product e)
        {
            cmd.Parameters.Add("@Product_ID", SqlDbType.Int).Value = e.ProductID;
            cmd.Parameters.Add("@ProductNumber", SqlDbType.Int).Value = e.ProductNumber;
            cmd.Parameters.Add("@ProductName", SqlDbType.NVarChar, 255).Value = e.ProductName;
            cmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = e.Price;
            cmd.Parameters.Add("@Size", SqlDbType.NVarChar, 255).Value = e.Size;
            cmd.Parameters.Add("@Colour", SqlDbType.NVarChar, 255).Value = e.Colour;
            cmd.Parameters.Add("ProductCatalogue_ID", SqlDbType.UniqueIdentifier)
                .Value = (object?)e.ProductCatalogueID ?? DBNull.Value;
        }

        protected override Guid GetKey(Product e)
        {
            throw new NotImplementedException();
        }

        protected override Product Map(IDataRecord r)
        {
            throw new NotImplementedException();
        }
    }
}
