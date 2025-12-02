using System.Windows.Input;
using Undy.Data.Repository;
using Undy.Models;
using Undy.ViewModels.Helpers;

namespace Undy.ViewModels
{
    public class TestSalesOrderViewModel : BaseViewModel
    {
        private readonly IBaseRepository<SalesOrder, Guid> _salesOrderRepo;
        private readonly IBaseRepository<Stock, Guid> _stockRepo;
        private readonly IBaseRepository<Product, Guid> _productRepo;
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
                if(SetProperty(ref _customerName, value));
            }
        }

        private string _selectedProduct;
        public string SelectedProduct {
            get => _selectedProduct;
            set {
                if(SetProperty(ref _selectedProduct, value));
            }
        }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if(SetProperty(ref _quantity, value));
            }
        }

        public ICommand ConfirmCommand { get; }

        public TestSalesOrderViewModel(IBaseRepository<SalesOrder, Guid> salesOrderRepo, IBaseRepository<Stock, Guid> stockRepo, IBaseRepository<Product, Guid> productRepo) {
            _salesOrderRepo = salesOrderRepo;
            _stockRepo = stockRepo;
            _productRepo = productRepo;
            //_salesOrderService = salesOrderService;

            ConfirmCommand = new RelayCommand(CreateSalesOrder);
        }

        private async void CreateSalesOrder() {
            var prod
        }
    }
}
