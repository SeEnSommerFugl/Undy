namespace Undy.Features.Products.Dialog
{
    public enum ProductDialogMode { Add, Edit }

    public partial class ProductDialog : Window
    {
        private readonly ProductDialogMode _mode;
        private readonly Product? _original;

        private readonly Func<Product, Task> _addAsync;
        private readonly Func<Product, Task> _editAsync;

        public ProductDialog(ProductDialogMode mode, Product? original, Func<Product, Task> addAsync, Func<Product, Task> editAsync)
        {
            InitializeComponent();

            _mode = mode;
            _original = original;

            _addAsync = addAsync ?? throw new ArgumentNullException(nameof(addAsync));
            _editAsync = editAsync ?? throw new ArgumentNullException(nameof(editAsync));

            ApplyModeUi();
            PrefillFields();
        }

        private void ApplyModeUi()
        {
            if (_mode == ProductDialogMode.Add)
            {
                Title = "Tilføj produkt";
                HeaderText.Text = "Tilføj produkt";
                SaveButton.Content = "Create";
            }
            else
            {
                Title = "Redigér produkt";
                HeaderText.Text = "Redigér produkt";
                SaveButton.Content = "Save";
            }
        }

        private void PrefillFields()
        {
            ErrorText.Text = "";

            if (_mode != ProductDialogMode.Edit || _original is null)
                return;

            ProductNumberBox.Text = _original.ProductNumber;
            ProductNameBox.Text = _original.ProductName;
            PriceBox.Text = _original.Price.ToString(CultureInfo.CurrentCulture);
            SizeBox.Text = _original.Size;
            ColourBox.Text = _original.Colour;
            NumberInStockBox.Text = _original.NumberInStock.ToString(CultureInfo.CurrentCulture);
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Text = "";

            if (!ProductInputValidator.TryValidate(
                    ProductNumberBox.Text,
                    ProductNameBox.Text,
                    PriceBox.Text,
                    SizeBox.Text,
                    ColourBox.Text,
                    NumberInStockBox.Text,
                    out var input,
                    out var err))
            {
                ErrorText.Text = err;
                return;
            }

            try
            {
                if (_mode == ProductDialogMode.Add)
                {
                    var created = new Product
                    {
                        ProductID = Guid.NewGuid(),
                        ProductNumber = input!.ProductNumber,
                        ProductName = input.ProductName,
                        Price = input.Price,
                        Size = input.Size,
                        Colour = input.Colour,
                        NumberInStock = input.NumberInStock
                    };

                    await _addAsync(created);
                    DialogResult = true;
                    return;
                }

                if (_original is null)
                {
                    ErrorText.Text = "Ingen originalt produkt at redigere.";
                    return;
                }

                var updated = new Product
                {
                    ProductID = _original.ProductID,     // source of truth
                    ProductNumber = input.ProductNumber, // user-editable
                    ProductName = input.ProductName,
                    Price = input.Price,
                    Size = input.Size,
                    Colour = input.Colour,
                    NumberInStock = input.NumberInStock
                };

                await _editAsync(updated); // => repo.UpdateAsync(updated)
                DialogResult = true;
            }
            catch (Exception ex)
            {
                // RAISERROR/THROW fra SQL ender her (SqlException.Message)
                ErrorText.Text = ex.Message;
            }
        }
    }
}
