namespace Undy.Models
{
    public class Product
    {
        public Guid ProductID { get; set; }
        public int ProductNumber { get; set; }
        public string Productname { get; set; }
        public decimal Price { get; set; }
        public int InStock { get; set; }
        public int Status { get; set; }
        public int MinimumStockLimit { get; set; }
        public Guid ProductCatalogueID { get; set; }


    }
}
