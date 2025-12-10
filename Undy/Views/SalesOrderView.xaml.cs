using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
            // Ensure the click originated from a GridViewColumnHeader
            if (e.OriginalSource is not GridViewColumnHeader header ||
                header.Column is null)
                return;

            // Get the CollectionView of the ListView's ItemsSource
            var view = CollectionViewSource.GetDefaultView(SalesListView.ItemsSource);
            if (view == null)
                return;

            // Extract the property name from DisplayMemberBinding
            var binding = header.Column.DisplayMemberBinding as Binding;
            var sortProperty = binding?.Path?.Path;

            if (string.IsNullOrEmpty(sortProperty))
                return;

            // Toggle sort direction if the same column is clicked again
            if (_lastSortProperty == sortProperty)
            {
                _lastSortDirection =
                    _lastSortDirection == ListSortDirection.Ascending
                        ? ListSortDirection.Descending
                        : ListSortDirection.Ascending;
            }
            else
            {
                // New column clicked: reset to ascending order
                _lastSortProperty = sortProperty;
                _lastSortDirection = ListSortDirection.Ascending;
            }

            // Apply the sorting
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(
                new SortDescription(sortProperty, _lastSortDirection));
        }
    }
}
