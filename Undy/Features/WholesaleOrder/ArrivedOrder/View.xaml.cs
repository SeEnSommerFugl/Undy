using System.Windows.Controls;
using Undy.ViewModels;

namespace Undy.Views
{
    public partial class IncomingWholesaleOrderView : UserControl
    {
        public IncomingWholesaleOrderView()
        {
            InitializeComponent();

            // Når viewet er loadet, så bed den eksisterende VM om at hente data
            Loaded += async (_, __) =>
            {
                if (DataContext is IncomingWholesaleOrderViewModel vm)
                {
                    await vm.LoadAsync();
                }
            };
        }
    }
}
