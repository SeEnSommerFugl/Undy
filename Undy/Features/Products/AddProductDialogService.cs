// Fil: Undy/Undy/Features/Products/AddProduct/AddProductDialogService.cs
namespace Undy.Features.Products.AddProduct
{
    public interface IAddProductDialogService
    {
        /// <summary>
        /// Shows a low-fi dialog. Returns a Product if user confirmed and validation passed; otherwise null.
        /// </summary>
        Product? ShowDialog(Window? owner = null);
    }

    public sealed class AddProductDialogService : IAddProductDialogService
    {
        public Product? ShowDialog(Window? owner = null)
        {
            var win = BuildLowFiAddProductWindow(
                out var tbNumber,
                out var tbName,
                out var tbPrice,
                out var tbSize,
                out var tbColour,
                out var tbStock);

            win.Owner = owner ?? Application.Current?.MainWindow;

            var ok = win.ShowDialog();
            if (ok != true) return null;

            var productNumber = (tbNumber.Text ?? string.Empty).Trim();
            var productName = (tbName.Text ?? string.Empty).Trim();
            var size = (tbSize.Text ?? string.Empty).Trim();
            var colour = (tbColour.Text ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(productNumber) || string.IsNullOrWhiteSpace(productName))
            {
                MessageBox.Show(
                    "Produktnummer og produktnavn skal udfyldes.",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return null;
            }

            if (!TryParseDecimal(tbPrice.Text, out var price) || price < 0)
            {
                MessageBox.Show(
                    "Pris skal være et gyldigt tal (>= 0).",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return null;
            }

            if (!int.TryParse((tbStock.Text ?? string.Empty).Trim(), NumberStyles.Integer, CultureInfo.CurrentCulture, out var stock) || stock < 0)
            {
                MessageBox.Show(
                    "Antal på lager skal være et gyldigt heltal (>= 0).",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return null;
            }

            return new Product
            {
                ProductID = Guid.NewGuid(),
                ProductNumber = productNumber,
                ProductName = productName,
                Price = price,
                Size = size,
                Colour = colour,
                NumberInStock = stock
            };
        }

        private static bool TryParseDecimal(string? input, out decimal value)
        {
            var s = (input ?? string.Empty).Trim();

            // CurrentCulture først (DK: 139,00) + fallback til invariant (139.00)
            if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.CurrentCulture, out value))
                return true;

            return decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
        }

        private static Window BuildLowFiAddProductWindow(
            out TextBox tbNumber,
            out TextBox tbName,
            out TextBox tbPrice,
            out TextBox tbSize,
            out TextBox tbColour,
            out TextBox tbStock)
        {
            tbNumber = new TextBox { MinWidth = 260 };
            tbName = new TextBox { MinWidth = 260 };
            tbPrice = new TextBox { MinWidth = 260 };
            tbSize = new TextBox { MinWidth = 260 };
            tbColour = new TextBox { MinWidth = 260 };
            tbStock = new TextBox { MinWidth = 260 };

            var grid = new Grid { Margin = new Thickness(16) };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            void AddRow(string label, Control input, int row)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var lbl = new TextBlock
                {
                    Text = label,
                    Margin = new Thickness(0, 6, 10, 6),
                    VerticalAlignment = VerticalAlignment.Center
                };

                Grid.SetRow(lbl, row);
                Grid.SetColumn(lbl, 0);

                Grid.SetRow(input, row);
                Grid.SetColumn(input, 1);

                grid.Children.Add(lbl);
                grid.Children.Add(input);
            }

            AddRow("Produktnummer:", tbNumber, 0);
            AddRow("Produktnavn:", tbName, 1);
            AddRow("Pris:", tbPrice, 2);
            AddRow("Størrelse:", tbSize, 3);
            AddRow("Farve:", tbColour, 4);
            AddRow("Antal på lager:", tbStock, 5);

            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var buttons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 14, 0, 0)
            };

            var btnCancel = new Button { Content = "Cancel", MinWidth = 90, Margin = new Thickness(0, 0, 10, 0) };
            var btnCreate = new Button { Content = "Create", MinWidth = 90, IsDefault = true };

            var win = new Window
            {
                Title = "Tilføj produkt",
                Width = 520,
                Height = 320,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Content = new DockPanel()
            };

            btnCancel.Click += (_, __) => win.DialogResult = false;
            btnCreate.Click += (_, __) => win.DialogResult = true;

            buttons.Children.Add(btnCancel);
            buttons.Children.Add(btnCreate);

            Grid.SetRow(buttons, 6);
            Grid.SetColumnSpan(buttons, 2);
            grid.Children.Add(buttons);

            ((DockPanel)win.Content).Children.Add(grid);

            return win;
        }
    }
}
