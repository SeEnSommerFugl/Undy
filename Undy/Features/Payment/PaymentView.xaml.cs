namespace Undy.Views
{
    public partial class PaymentView : UserControl
    {
        private PaymentViewModel? ViewModel => DataContext as PaymentViewModel;

        public PaymentView()
        {
            InitializeComponent();
        }

        private void PaymentCheckBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.Tag is Guid SalesOrderID)
            {
                // Ask HashSet if this booth is selected
                checkBox.IsChecked = ViewModel?.IsSalesOrderSelected(SalesOrderID) ?? false;
            }
        }

        /// <summary>
        /// When user checks checkbox, add to HashSet
        /// </summary>
        private void PaymentCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.Tag is Guid SalesOrderID)
            {
                // Add to HashSet = super fast operation
                ViewModel?.SetSalesOrderSelection(SalesOrderID, true);
            }
        }

        /// <summary>
        /// When user unchecks checkbox, remove from HashSet
        /// </summary>
        private void PaymentCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.Tag is Guid SalesOrderID)
            {
                // Remove from HashSet = super fast operation
                ViewModel?.SetSalesOrderSelection(SalesOrderID, false);
            }
        }

        private void PaymentCheckBox_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                // Clear state when container is being recycled
                checkBox.IsChecked = false;
            }
        }
    }
}
