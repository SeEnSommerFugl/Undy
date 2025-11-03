namespace Undy.Models
{
    public class Stock
    {
        public Guid StockID { get; set; }
        public int InStock { get; set; }
        public int Status { get; set; }
        public int MinimumStockLimit { get; set; }
    }
}
