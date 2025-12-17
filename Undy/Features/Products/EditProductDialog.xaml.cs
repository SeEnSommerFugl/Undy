namespace Undy.Views
{
    public partial class EditProductDialog : Window
    {
        private readonly Product _original;

        public Product? UpdatedProduct { get; private set; }

        public EditProductDialog(Product product)
        {
            InitializeComponent();
            _original = product ?? throw new ArgumentNullException(nameof(product));

            ProductNumberBox.Text = _original.ProductNumber;
            ProductNameBox.Text = _original.ProductName;
            PriceBox.Text = _original.Price.ToString(CultureInfo.CurrentCulture);
            SizeBox.Text = _original.Size;
            ColourBox.Text = _original.Colour;
            NumberInStockBox.Text = _original.NumberInStock.ToString(CultureInfo.CurrentCulture);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var productNumber = (ProductNumberBox.Text ?? string.Empty).Trim();
            var productName = (ProductNameBox.Text ?? string.Empty).Trim();
            var size = (SizeBox.Text ?? string.Empty).Trim();
            var colour = (ColourBox.Text ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(productNumber) || string.IsNullOrWhiteSpace(productName))
            {
                MessageBox.Show("Produktnummer og produktnavn skal udfyldes.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TryParseDecimal(PriceBox.Text, out var price) || price < 0)
            {
                MessageBox.Show("Pris skal være et gyldigt tal (>= 0).", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse((NumberInStockBox.Text ?? string.Empty).Trim(), NumberStyles.Integer, CultureInfo.CurrentCulture, out var stock) || stock < 0)
            {
                MessageBox.Show("Antal på lager skal være et gyldigt heltal (>= 0).", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            UpdatedProduct = new Product
            {
                ProductID = _original.ProductID,
                ProductNumber = productNumber,
                ProductName = productName,
                Price = price,
                Size = size,
                Colour = colour,
                NumberInStock = stock
            };

            DialogResult = true;
        }

        private static bool TryParseDecimal(string? input, out decimal value)
        {
            var s = (input ?? string.Empty).Trim();
            if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.CurrentCulture, out value))
                return true;
            return decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
        }
    }
}
