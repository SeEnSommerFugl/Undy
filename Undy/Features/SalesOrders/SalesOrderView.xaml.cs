using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Undy.Features.ViewModel;
using Undy.Models;

namespace Undy.Views
{
    public partial class SalesOrderView : UserControl
    {
        private GridViewColumnHeader? _lastHeaderClicked;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;

        public SalesOrderView()
        {
            InitializeComponent();
        }

        // Loads order details on double-click (not on SelectionChanged).
        private async void SalesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is not SalesOrderViewModel vm)
                return;

            if (sender is not ListView listView)
                return;

            if (listView.SelectedItem is not SalesOrder selectedOrder)
                return;

            await vm.LoadOrderDetailsAsync(selectedOrder);
        }

        // Sorts the ListView when a GridViewColumnHeader is clicked (EventSetter in XAML).
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not GridViewColumnHeader headerClicked)
                return;

            if (headerClicked.Column?.DisplayMemberBinding is not Binding binding ||
                binding.Path is null ||
                string.IsNullOrWhiteSpace(binding.Path.Path))
                return;

            var sortBy = binding.Path.Path;

            var direction = ListSortDirection.Ascending;
            if (_lastHeaderClicked == headerClicked)
            {
                direction = _lastDirection == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }

            var view = CollectionViewSource.GetDefaultView(SalesListView.ItemsSource);
            if (view is null)
                return;

            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription(sortBy, direction));
            view.Refresh();

            _lastHeaderClicked = headerClicked;
            _lastDirection = direction;
        }
    }
}
