using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using Undy.Models;
using Undy.ViewModels;

namespace Undy.Views
{
    public partial class SalesOrderView : UserControl
    {
        // Stores the last property that was sorted
        private string? _lastSortProperty;

        // Stores the last sort direction (Ascending / Descending)
        private ListSortDirection _lastSortDirection = ListSortDirection.Ascending;

        public SalesOrderView()
        {
            InitializeComponent();
        }

        // Handles click on GridView column headers to sort the ListView
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            // Ensure the click came from a GridViewColumnHeader
            if (e.OriginalSource is not GridViewColumnHeader header ||
                header.Column is null)
            {
                return;
            }

            // Get CollectionView from the ListView source
            var view = CollectionViewSource.GetDefaultView(SalesListView.ItemsSource);
            if (view == null)
            {
                return;
            }

            // Get property name from binding
            var binding = header.Column.DisplayMemberBinding as Binding;
            var sortProperty = binding?.Path?.Path;

            if (string.IsNullOrEmpty(sortProperty))
            {
                return;
            }

            // Toggle direction if same column is clicked again
            if (_lastSortProperty == sortProperty)
            {
                _lastSortDirection =
                    _lastSortDirection == ListSortDirection.Ascending
                        ? ListSortDirection.Descending
                        : ListSortDirection.Ascending;
            }
            else
            {
                _lastSortProperty = sortProperty;
                _lastSortDirection = ListSortDirection.Ascending;
            }

            // Apply sorting
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(
                new SortDescription(sortProperty, _lastSortDirection));
        }

        // Handles double click on a sales order
        private void SalesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Get view model
            if (DataContext is not SalesOrderViewModel viewModel)
            {
                return;
            }

            // Get selected order
            if (SalesListView.SelectedItem is not SalesOrderDisplay selectedOrder)
            {
                return;
            }

            // Load order details into DataGrid
            viewModel.LoadOrderDetails(selectedOrder);
        }
    }
}
