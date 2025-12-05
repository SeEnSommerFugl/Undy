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
    public class SalesOrderViewModel : BaseViewModel
    {
        private readonly IBaseRepository<SalesOrderDisplay, Guid> _salesDisplayRepo;

        private SalesOrderDisplay _selectedSalesOrder;

        public ObservableCollection<SalesOrderDisplay> SalesDisplay => _salesDisplayRepo.Items;
        public ICollectionView SaleView { get; }

        
        public Action<SalesOrderDisplay> PaymentRequested { get; set; }

        public SalesOrderViewModel(IBaseRepository<SalesOrderDisplay, Guid> salesDisplayRepo)
        {
            _salesDisplayRepo = salesDisplayRepo;

            SaleView = CollectionViewSource.GetDefaultView(SalesDisplay);

            OpenPaymentCommand = new RelayCommand(
                _ => OnOpenPayment(),
                _ => CanOpenPayment()
            );
        }

        // ---- Selected item ----
        public SalesOrderDisplay SelectedSalesOrder
        {
            get => _selectedSalesOrder;
            set
            {
                _selectedSalesOrder = value;
                OnPropertyChanged();

                (OpenPaymentCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        // ---- Command ----
        public ICommand OpenPaymentCommand { get; }

        private bool CanOpenPayment()
        {
            return SelectedSalesOrder != null;
        }

        private void OnOpenPayment()
        {
            if (SelectedSalesOrder == null)
                return;

            PaymentRequested?.Invoke(SelectedSalesOrder);
        }
    }

}