namespace Undy.Views
{
    public partial class SalesOrderView : UserControl
    {
        private string? _lastSortProperty;
        private ListSortDirection _lastSortDirection = ListSortDirection.Descending;

        public SalesOrderView()
        {
            InitializeComponent();

            Loaded += SalesOrderView_Loaded;
        }

        private void SalesOrderView_Loaded(object sender, RoutedEventArgs e)
        {
            // Hook header clicks once
            SalesListView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(OnGridViewHeaderClick));

            // Default sort: newest order number first
            ApplySort("SalesOrderNumber", ListSortDirection.Descending);

            // Default select first item (after sort)
            if (SalesListView.Items.Count > 0 && SalesListView.SelectedItem == null)
                SalesListView.SelectedIndex = 0;
        }

        private void OnGridViewHeaderClick(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is not GridViewColumnHeader header)
                return;

            // Ignore padding clicks etc.
            if (header.Role == GridViewColumnHeaderRole.Padding)
                return;

            // Resolve bound property name from the column's DisplayMemberBinding
            var column = header.Column;
            if (column?.DisplayMemberBinding is not Binding binding || binding.Path == null)
                return;

            var property = binding.Path.Path;
            if (string.IsNullOrWhiteSpace(property))
                return;

            // Toggle direction if same column clicked again
            var direction = ListSortDirection.Ascending;
            if (_lastSortProperty == property)
            {
                direction = _lastSortDirection == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }

            ApplySort(property, direction);
        }

        private void ApplySort(string property, ListSortDirection direction)
        {
            var view = CollectionViewSource.GetDefaultView(SalesListView.ItemsSource);
            if (view == null)
                return;

            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription(property, direction));
            view.Refresh();

            _lastSortProperty = property;
            _lastSortDirection = direction;
        }

        // Double-click loads the selected order's lines
        private async void SalesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is not SalesOrderViewModel vm)
                return;

            if (SalesListView.SelectedItem is not SalesOrder order)
                return;

            try
            {
                await vm.LoadOrderDetailsAsync(order);
            }
            catch
            {
                // Keep view stable even if SQL call fails; handle/log if you have a logger
            }
        }
    }
}
