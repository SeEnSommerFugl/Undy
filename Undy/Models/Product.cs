namespace Undy.Models
{
    public class Product
    {
        public Guid ProductID { get; set; }
        public string ProductNumber { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
        public string Colour { get; set; }
        public int NumberInStock { get; set; }


    }
}
