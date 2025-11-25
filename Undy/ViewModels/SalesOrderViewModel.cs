using System.Collections.ObjectModel;
using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels
{
    public class SalesOrderViewModel : BaseViewModel
    {
        private readonly SalesOrderDisplayDBRepository _salesDisplayRepo;

        public ObservableCollection<SalesOrderDisplay> SalesDisplay { get; set; }

        public SalesOrderViewModel(SalesOrderDisplayDBRepository salesDisplayRepo)
        {
            _salesDisplayRepo = salesDisplayRepo;
            LoadSalesOrders();
            //public ObservableCollection<SalesOrder> SalesOrders { get; set; }
        }

        private async void LoadSalesOrders() {
            await _salesDisplayRepo.InitializeAsync();
            SalesDisplay = _salesDisplayRepo.Items;
            OnPropertyChanged(nameof(SalesDisplay));
        }
    }
}
