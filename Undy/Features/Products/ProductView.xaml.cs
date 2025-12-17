using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Undy.Views
{
    public partial class ProductView : UserControl
    {
        public ProductView()
        {
            InitializeComponent();
        }

        // Enter should commit the edit (DB update happens in VM command).
        private void ProductsGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (DataContext is not ProductViewModel vm) return;

            // Only commit if we are currently editing.
            if (ProductsGrid.CurrentCell.Column == null) return;

            // Force WPF to push the edited value into the bound object.
            ProductsGrid.CommitEdit(DataGridEditingUnit.Cell, true);
            ProductsGrid.CommitEdit(DataGridEditingUnit.Row, true);

            // Tell VM to persist the currently edited row.
            vm.CommitEditedQuantityCommand.Execute(null);

            e.Handled = true;
        }
    }
}
