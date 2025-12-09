using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using Undy.Data.Repository;
using Undy.Models;
using Undy.ViewModels.Helpers;

namespace Undy.ViewModels
{
    public class SalesOrderViewModel : BaseViewModel {
        private readonly IBaseRepository<SalesOrderDisplay, Guid> _salesDisplayRepo;

        public ObservableCollection<SalesOrderDisplay> SalesDisplay => _salesDisplayRepo.Items;
        public ICollectionView SaleView { get; }

        public SalesOrderViewModel(IBaseRepository<SalesOrderDisplay, Guid> salesDisplayRepo) {
            _salesDisplayRepo = salesDisplayRepo;
            SaleView = CollectionViewSource.GetDefaultView(SalesDisplay);
            SaleView.SortDescriptions.Add(new SortDescription("SalesDate", ListSortDirection.Descending));
        }
    }
}