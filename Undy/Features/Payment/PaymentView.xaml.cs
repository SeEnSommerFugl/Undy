using System.Windows.Controls;
using Undy.ViewModels;   

namespace Undy.Views
{
    public partial class PaymentView : UserControl
    {
        public PaymentView()
        {
            InitializeComponent();

            Loaded += async (_, __) =>
            {
                if (DataContext is PaymentViewModel vm)
                {
                    await vm.LoadUnpaidOrdersAsync();
                }
            };
        }
    }
}
