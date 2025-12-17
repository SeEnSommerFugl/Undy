namespace Undy.Views
{
    public partial class ProductView : UserControl
    {
        public ProductView()
        {
            InitializeComponent();
        }

        private void ProductsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ListView lv) return;
            if (lv.SelectedItem == null) return;

            // Forhindrer at dobbeltklik på scrollbar/blankt område trigger
            if (e.OriginalSource is DependencyObject depObj)
            {
                var item = ItemsControl.ContainerFromElement(lv, depObj) as ListViewItem;
                if (item == null) return;
            }

            // Kør samme command som din (gamle) Edit-knap brugte
            var vm = DataContext;
            if (vm == null) return;

            // 1) Find OpenEditCommand på VM (samme navn som din knap brugte)
            var cmdProp = vm.GetType().GetProperty("OpenEditCommand");
            if (cmdProp?.GetValue(vm) is not ICommand cmd) return;

            var param = lv.SelectedItem;

            if (cmd.CanExecute(param))
                cmd.Execute(param);
        }
    }
}
