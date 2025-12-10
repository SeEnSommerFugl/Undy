using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;
using Undy.Data.Repository;
using Undy.Models;
using Undy.Features.Helpers;
using Undy.Features.Base;
using Undy.Features.SalesOrders;

namespace Undy.Features.SalesOrders.Demo
{
    public class TestSalesOrderViewModel : BaseViewModel
    {
        private readonly IBaseRepository<SalesOrder, Guid> _salesOrderRepo;
        private readonly IBaseRepository<Product, Guid> _productRepo;
        private readonly IBaseRepository<ProductSalesOrder, Guid> _productSalesOrderRepo;
        private readonly IBaseRepository<Customer, Guid> _customerRepo;
        private readonly SalesOrderService _salesOrderService;
        private readonly ICollectionView _testSalesOrderView;


        public ObservableCollection<Product> Products => _productRepo.Items;
        public ObservableCollection<Customer> Customers => _customerRepo.Items;
        public ObservableCollection<SalesOrderLineViewModel> SalesOrderLines { get; } = new();
        public ICollectionView TestSalesOrderView => _testSalesOrderView;
        public ICollectionView SortedCustomers { get; }

        public TestSalesOrderViewModel(IBaseRepository<SalesOrder, Guid> salesOrderRepo, IBaseRepository<Product, Guid> productRepo, IBaseRepository<ProductSalesOrder, Guid> productSalesOrderRepo, IBaseRepository<Customer, Guid> customerRepo) {
            _salesOrderRepo = salesOrderRepo;
            _productRepo = productRepo;
            _productSalesOrderRepo = productSalesOrderRepo;
            _customerRepo = customerRepo;
            _salesOrderService = new SalesOrderService();
            _testSalesOrderView = CollectionViewSource.GetDefaultView(Products);

            ConfirmCommand = new RelayCommand(async _ => await CreateSalesOrderAsync());
            AddProductCommand = new RelayCommand(_ => AddProduct());
            RemoveSalesOrderLineCommand = new RelayCommand(salesOrderLine => RemoveSalesOrderLine(salesOrderLine as SalesOrderLineViewModel));

            SortedCustomers = CollectionViewSource.GetDefaultView(_customerRepo.Items);
            SortedCustomers.SortDescriptions.Add(new SortDescription(nameof(Customer.CustomerNumber), ListSortDirection.Ascending));
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

        private Product _selectedProduct;
        public Product SelectedProduct {
            get => _selectedProduct;
            set {
                if(SetProperty(ref _selectedProduct, value));
            }
        }

        private Customer _selectedCustomer;
        public Customer SelectedCustomer {
            get => _selectedCustomer;
            set {
                if(SetProperty(ref _selectedCustomer, value));
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
            if (SalesOrderLines.Count == 0 || SelectedCustomer == null) {
                return;
            }

            var salesOrder = new SalesOrder {
                SalesOrderID = Guid.NewGuid(),
                CustomerID = SelectedCustomer.CustomerID,
                OrderStatus = "Afventer",
                PaymentStatus = "Afventer",
                SalesDate = DateOnly.FromDateTime(DateTime.Now)
            };
            await _salesOrderRepo.AddAsync(salesOrder);

            var SalesOrderLineList = new List<ProductSalesOrder>();
            var salesOrderLineProducts = SalesOrderLines.Select(sl => new ProductSalesOrder
            {
                SalesOrderID = salesOrder.SalesOrderID,
                ProductNumber = sl.Product.ProductNumber,
                UnitPrice = sl.UnitPrice,
                Quantity = sl.Quantity,
            });

            SalesOrderLineList.AddRange(salesOrderLineProducts);
            await _productSalesOrderRepo.AddRangeAsync2(salesOrderLineProducts);
            //.ToList();


            //await _salesOrderService.CreateSalesOrderWithProducts(salesOrder,  salesOrderLineProducts);
        }
    }
}
