using System.Windows.Controls;
using Undy.Data.Repository;
using Undy.ViewModels;

namespace Undy.Views
{
    public partial class IncomingWholesaleOrderView : UserControl
    {
        private readonly IncomingWholesaleOrderViewModel _viewModel;

        public IncomingWholesaleOrderView()
        {
            InitializeComponent();

            // 1. Opret repos
            var wholesaleRepo = new WholesaleOrderDBRepository();
            var productRepo = new ProductDBRepository();                    
            var productWholesaleRepo = new ProductWholesaleOrderDBRepository();

            // 2. Opret ViewModel
            _viewModel = new IncomingWholesaleOrderViewModel(
                wholesaleRepo,
                productRepo,
                productWholesaleRepo);

            // 3. Sæt DataContext
            DataContext = _viewModel;

            // 4. Hent data når viewet er loadet
            Loaded += async (_, __) => await _viewModel.LoadAsync();
        }
    }
}
