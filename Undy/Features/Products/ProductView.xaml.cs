using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Undy.Features.Products
{
    public partial class ProductView : UserControl
    {
        public ProductView()
        {
            InitializeComponent();
        }

        // 1) Click Edit: select row + focus "Antal på lager" cell + begin edit
        private void EditQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;

            // The row item is the Button's DataContext
            var item = btn.DataContext;
            if (item is null) return;

            // Tell VM which product is being edited (if VM supports it)
            if (DataContext is ProductViewModel vm && vm.BeginEditQuantityCommand?.CanExecute(item) == true)
                vm.BeginEditQuantityCommand.Execute(item);

            ProductsGrid.SelectedItem = item;
            ProductsGrid.ScrollIntoView(item);

            // Focus the "Antal på lager" column cell
            ProductsGrid.CurrentCell = new DataGridCellInfo(item, NumberInStockColumn);
            ProductsGrid.BeginEdit();

            // Try to focus the editing TextBox
            ProductsGrid.Dispatcher.InvokeAsync(() =>
            {
                var cell = GetCell(ProductsGrid, ProductsGrid.SelectedIndex, NumberInStockColumn.DisplayIndex);
                if (cell?.Content is TextBox tb)
                {
                    tb.Focus();
                    tb.SelectAll();
                }
            });
        }

        // 2) Enter: commit edit + persist to DB via VM command
        private void ProductsGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (DataContext is not ProductViewModel vm) return;

            // Only commit if we are editing the stock column
            if (ProductsGrid.CurrentCell.Column != NumberInStockColumn) return;

            ProductsGrid.CommitEdit(DataGridEditingUnit.Cell, true);
            ProductsGrid.CommitEdit(DataGridEditingUnit.Row, true);

            if (vm.CommitEditedQuantityCommand?.CanExecute(null) == true)
                vm.CommitEditedQuantityCommand.Execute(null);

            e.Handled = true;
        }

        // Helpers to locate a DataGridCell
        private static DataGridRow? GetRow(DataGrid grid, int index)
        {
            return (DataGridRow?)grid.ItemContainerGenerator.ContainerFromIndex(index)
                   ?? (DataGridRow?)grid.Dispatcher.Invoke(() =>
                   {
                       grid.UpdateLayout();
                       grid.ScrollIntoView(grid.Items[index]);
                       return grid.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
                   });
        }

        private static DataGridCell? GetCell(DataGrid grid, int rowIndex, int columnIndex)
        {
            var row = GetRow(grid, rowIndex);
            if (row is null) return null;

            var presenter = FindVisualChild<DataGridCellsPresenter>(row);
            if (presenter is null)
            {
                row.ApplyTemplate();
                presenter = FindVisualChild<DataGridCellsPresenter>(row);
                if (presenter is null) return null;
            }

            var cell = presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
            if (cell is null)
            {
                grid.ScrollIntoView(row, grid.Columns[columnIndex]);
                cell = presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
            }

            return cell;
        }

        private static T? FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child is T t) return t;

                var found = FindVisualChild<T>(child);
                if (found is not null) return found;
            }
            return null;
        }
    }
}
