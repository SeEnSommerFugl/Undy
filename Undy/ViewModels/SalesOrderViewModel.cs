using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels
{
    public class SalesOrderViewModel : BaseViewModel
    {
        private IBaseRepository<SalesOrderDisplay, Guid> _salesDisplayRepo;

        public ObservableCollection<SalesOrderDisplay> SalesDisplay => _salesDisplayRepo.Items;
        public ICollectionView SaleView { get;}

        public SalesOrderViewModel(IBaseRepository<SalesOrderDisplay, Guid> salesDisplayRepo)
        {
            _salesDisplayRepo = salesDisplayRepo;
            SaleView = CollectionViewSource.GetDefaultView(SalesDisplay);
        }
    }
}
