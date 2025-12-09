using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;
using Undy.Data.Repository;
using Undy.Models;
using Undy.ViewModels.Helpers;

namespace Undy.ViewModels
{
    public class TestSalesOrderViewModel : BaseViewModel
    {
        private readonly IBaseRepository<SalesOrder, Guid> _salesOrderRepo;
        private readonly IBaseRepository<Product, Guid> _productRepo;
        private readonly SalesOrderService _salesOrderService;
        private readonly ICollectionView _testSalesOrderView;


        public ObservableCollection<Product> Products => _productRepo.Items;
        public ObservableCollection<SalesOrderLineViewModel> SalesOrderLines { get; } = new();
        public ICollectionView TestSalesOrderView => _testSalesOrderView;

        public TestSalesOrderViewModel(IBaseRepository<SalesOrder, Guid> salesOrderRepo, IBaseRepository<Product, Guid> productRepo) {
            _salesOrderRepo = salesOrderRepo;
            _productRepo = productRepo;
            _salesOrderService = new SalesOrderService();
            _testSalesOrderView = CollectionViewSource.GetDefaultView(Products);

            ConfirmCommand = new RelayCommand(async _ => await CreateSalesOrderAsync());
            AddProductCommand = new RelayCommand(_ => AddProduct());
            RemoveSalesOrderLineCommand = new RelayCommand(salesOrderLine => RemoveSalesOrderLine(salesOrderLine as SalesOrderLineViewModel));
        }

        private SalesOrder _currentSalesOrder;
        public SalesOrder CurrentSalesOrder
        {
            get => _currentSalesOrder;
            set
            {
                if(SetProperty(ref _currentSalesOrder, value));
            }
        }

        private int _customerNumber;
        public int CustomerNumber
        {
            get => _customerNumber;
            set
            {
                if(SetProperty(ref _customerNumber, value));
            }
        }

        private Product _selectedProduct;
        public Product SelectedProduct {
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
        public ICommand RemoveSalesOrderLineCommand { get; }
        public ICommand AddProductCommand { get; }

        private void AddProduct() {
            if (SelectedProduct == null || Quantity <= 0) {
                return;
            }

            SalesOrderLines.Add(new SalesOrderLineViewModel(SelectedProduct, Quantity));
        }

        private void RemoveSalesOrderLine(SalesOrderLineViewModel salesOrderLine) {
            if(salesOrderLine != null) {
                SalesOrderLines.Remove(salesOrderLine);
            }
        }

        private async Task CreateSalesOrderAsync() {
            if (SalesOrderLines.Count == 0 || CustomerNumber <= 0) {
                return;
            }

            var salesOrder = new SalesOrder {
                CustomerNumber = CustomerNumber,
                OrderStatus = "Afventer Behandling",
                PaymentStatus = "Afventer Betaling",
                SalesDate = DateOnly.FromDateTime(DateTime.Now),
                TotalPrice = SalesOrderLines.Sum(sl => sl.SubTotal)
            };

            var salesOrderLineProducts = SalesOrderLines.Select(sl => new ProductSalesOrder {
                SalesOrderID = salesOrder.SalesOrderID,
                ProductID = sl.Product.ProductID,
                UnitPrice = sl.UnitPrice,
                Quantity = sl.Quantity,
            }).ToList();

            await _salesOrderService.CreateSalesOrderWithProducts(salesOrder,  salesOrderLineProducts);
        }
    }
}
