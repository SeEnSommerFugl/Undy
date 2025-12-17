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
            Size = r.IsDBNull(r.GetOrdinal("Size")) ? string.Empty : r.GetString(r.GetOrdinal("Size")),
            Colour = r.IsDBNull(r.GetOrdinal("Colour")) ? string.Empty : r.GetString(r.GetOrdinal("Colour")),
            NumberInStock = r.GetInt32(r.GetOrdinal("NumberInStock"))
        };

        protected override void BindInsert(SqlCommand cmd, Product e)
        {
            cmd.CommandType = CommandType.StoredProcedure;

            // SQL Parametres
            cmd.Parameters.Add("@ProductID", SqlDbType.UniqueIdentifier).Value = e.ProductID;
            cmd.Parameters.Add("@ProductNumber", SqlDbType.NVarChar, 255).Value = e.ProductNumber;
            cmd.Parameters.Add("@ProductName", SqlDbType.NVarChar, 255).Value = e.ProductName;

            var pPrice = cmd.Parameters.Add("@Price", SqlDbType.Decimal);
            pPrice.Precision = 10;
            pPrice.Scale = 2;
            pPrice.Value = e.Price;

            cmd.Parameters.Add("@Size", SqlDbType.NVarChar, 255).Value = (object?)e.Size ?? DBNull.Value;
            cmd.Parameters.Add("@Colour", SqlDbType.NVarChar, 255).Value = (object?)e.Colour ?? DBNull.Value;
            cmd.Parameters.Add("@NumberInStock", SqlDbType.Int).Value = e.NumberInStock;
        }

        // Fallback-only. When ProductNumber is editable, UI needs to use UpdateByProductNumberAsync(...)
        // So we can send the "old key" + "New value" correctly.
        protected override void BindUpdate(SqlCommand cmd, Product e)
        {
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@ProductNumber", SqlDbType.NVarChar, 255).Value = e.ProductNumber;
            cmd.Parameters.Add("@NewProductNumber", SqlDbType.NVarChar, 255).Value = DBNull.Value;

            cmd.Parameters.Add("@ProductName", SqlDbType.NVarChar, 255).Value = (object?)e.ProductName ?? DBNull.Value;

            var pPrice = cmd.Parameters.Add("@Price", SqlDbType.Decimal);
            pPrice.Precision = 10;
            pPrice.Scale = 2;
            pPrice.Value = e.Price;

            cmd.Parameters.Add("@Size", SqlDbType.NVarChar, 255).Value = (object?)e.Size ?? DBNull.Value;
            cmd.Parameters.Add("@Colour", SqlDbType.NVarChar, 255).Value = (object?)e.Colour ?? DBNull.Value;
            cmd.Parameters.Add("@NumberInStock", SqlDbType.Int).Value = e.NumberInStock;
        }

        /// <summary>
        /// Matches the current SQL Database.
        /// usp_Update_Product(
        ///   @ProductNumber, @NewProductNumber = NULL,
        ///   @ProductName = NULL, @Price = NULL, @Size = NULL, @Colour = NULL, @NumberInStock = NULL
        /// )
        /// </summary>
        public async Task UpdateByProductNumberAsync(string oldProductNumber, Product updated)
        {
            using var con = await DB.OpenConnection();
            using var cmd = new SqlCommand(SqlUpdate, con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@ProductNumber", SqlDbType.NVarChar, 255).Value = oldProductNumber;

            if (!string.Equals(oldProductNumber, updated.ProductNumber, StringComparison.Ordinal))
                cmd.Parameters.Add("@NewProductNumber", SqlDbType.NVarChar, 255).Value = updated.ProductNumber;
            else
                cmd.Parameters.Add("@NewProductNumber", SqlDbType.NVarChar, 255).Value = DBNull.Value;

            cmd.Parameters.Add("@ProductName", SqlDbType.NVarChar, 255).Value =
                string.IsNullOrWhiteSpace(updated.ProductName) ? DBNull.Value : updated.ProductName;

            var pPrice = cmd.Parameters.Add("@Price", SqlDbType.Decimal);
            pPrice.Precision = 10;
            pPrice.Scale = 2;
            pPrice.Value = updated.Price;

            cmd.Parameters.Add("@Size", SqlDbType.NVarChar, 255).Value =
                string.IsNullOrWhiteSpace(updated.Size) ? DBNull.Value : updated.Size;

            cmd.Parameters.Add("@Colour", SqlDbType.NVarChar, 255).Value =
                string.IsNullOrWhiteSpace(updated.Colour) ? DBNull.Value : updated.Colour;

            cmd.Parameters.Add("@NumberInStock", SqlDbType.Int).Value = updated.NumberInStock;

            await cmd.ExecuteNonQueryAsync();

            // Refresh UI list from vw_Products
            await ReloadItemsAsync();
        }
    }
}
