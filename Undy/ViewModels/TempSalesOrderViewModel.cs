using Undy.Data.Repository;
using Undy.Models;
using Undy.ViewModels.Helpers;

namespace Undy.ViewModels
{
    internal class TempSalesOrderViewModel : BaseViewModel
    {
        private readonly IBaseRepository<SalesOrderDBRepository, Guid> _salesOrderRepo;
        private readonly SalesOrderService _salesOrderService;

        private SalesOrder _currentSalesOrder;
        public SalesOrder CurrentSalesOrder
        {
            get => _currentSalesOrder;
            set
            {
                _currentSalesOrder = value;
                OnPropertyChanged();
            }
        }

        private string _customerName;
        public string CustomerName
        {
            get => _customerName;
            set
            {
                if (SetProperty(ref _customerName, value)) ;
            }
        }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
            }
        }



    }
}
