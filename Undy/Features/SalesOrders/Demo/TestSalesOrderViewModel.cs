using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Undy.Data.Repository;
using Undy.Models;

namespace Undy.Features.ViewModel
{
    public class TestSalesOrderViewModel : BaseViewModel
    {
        private readonly IBaseRepository<SalesOrder, Guid> _salesOrderRepo;
        private readonly IBaseRepository<Product, Guid> _productRepo;
        private readonly SalesOrderLineDBRepository _salesOrderLineRepo;
        private readonly IBaseRepository<Customer, Guid> _customerRepo;

        public ObservableCollection<Product> Products => _productRepo.Items;
        public ObservableCollection<Customer> Customers => _customerRepo.Items;

        // Bound in XAML -> must be public and the item type must be at least as accessible.
        public ObservableCollection<TestSalesOrderLineEntryViewModel> SalesOrderLines { get; } = new();

        public ICollectionView SortedCustomers { get; }

        private Customer? _selectedCustomer;
        public Customer? SelectedCustomer
        {
            get => _selectedCustomer;
            set => SetProperty(ref _selectedCustomer, value);
        }

        private Product? _selectedProduct;
        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        public ICommand ConfirmCommand { get; }
        public ICommand AddProductCommand { get; }
        public ICommand RemoveSalesOrderLineCommand { get; }

        public TestSalesOrderViewModel(
            IBaseRepository<SalesOrder, Guid> salesOrderRepo,
            IBaseRepository<Product, Guid> productRepo,
            SalesOrderLineDBRepository salesOrderLineRepo,
            IBaseRepository<Customer, Guid> customerRepo)
        {
            _salesOrderRepo = salesOrderRepo;
            _productRepo = productRepo;
            _salesOrderLineRepo = salesOrderLineRepo;
            _customerRepo = customerRepo;

            ConfirmCommand = new RelayCommand(async _ => await CreateSalesOrderAsync());
            AddProductCommand = new RelayCommand(_ => AddProduct(), _ => CanAddProduct());
            RemoveSalesOrderLineCommand = new RelayCommand(line =>
                RemoveSalesOrderLine(line as TestSalesOrderLineEntryViewModel));

            SortedCustomers = CollectionViewSource.GetDefaultView(_customerRepo.Items);
            SortedCustomers.SortDescriptions.Add(
                new SortDescription(nameof(Customer.CustomerNumber), ListSortDirection.Ascending));
        }

        private bool CanAddProduct()
        {
            return SelectedProduct != null && Quantity > 0;
        }

        private void AddProduct()
        {
            if (SelectedProduct == null)
                return;

            var existing = SalesOrderLines.FirstOrDefault(x => x.ProductID == SelectedProduct.ProductID);
            if (existing != null)
            {
                existing.Quantity += Quantity;
                return;
            }

            // XAML expects Product.ProductName + Product.Size, so we store the full Product.
            SalesOrderLines.Add(new TestSalesOrderLineEntryViewModel
            {
                ProductID = SelectedProduct.ProductID,
                Product = SelectedProduct,
                UnitPrice = SelectedProduct.Price,
                Quantity = Quantity
            });

            Quantity = 0;
        }

        private void RemoveSalesOrderLine(TestSalesOrderLineEntryViewModel? line)
        {
            if (line == null) return;
            SalesOrderLines.Remove(line);
        }

        private async Task CreateSalesOrderAsync()
        {
            if (SelectedCustomer == null)
                return;

            var total = SalesOrderLines.Sum(l => l.Quantity * l.UnitPrice);

            var newOrder = new SalesOrder
            {
                SalesOrderID = Guid.NewGuid(),
                CustomerID = SelectedCustomer.CustomerID,
                SalesDate = DateOnly.FromDateTime(DateTime.Now),
                OrderStatus = "Afventer",
                PaymentStatus = "Afventer",
                ShippedDate = null,
                TotalPrice = total
            };

            await _salesOrderRepo.AddAsync(newOrder);

            // Repository insert needs ids/prices/qty, not Product object.
            var lines = SalesOrderLines.Select(l => new SalesOrderLine
            {
                SalesOrderID = newOrder.SalesOrderID,
                ProductID = l.ProductID,
                ProductName = l.Product?.ProductName ?? string.Empty,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice
            }).ToList();

            await _salesOrderLineRepo.AddRangeAsync(lines);

            SalesOrderLines.Clear();
            SelectedCustomer = null;
            SelectedProduct = null;
            Quantity = 0;
        }

        // Public because SalesOrderLines is public and bound in XAML.
        public class TestSalesOrderLineEntryViewModel : BaseViewModel
        {
            private Guid _productId;
            public Guid ProductID
            {
                get => _productId;
                set => SetProperty(ref _productId, value);
            }

            private Product? _product;
            public Product? Product
            {
                get => _product;
                set => SetProperty(ref _product, value);
            }

            private int _quantity;
            public int Quantity
            {
                get => _quantity;
                set => SetProperty(ref _quantity, value);
            }

            private decimal _unitPrice;
            public decimal UnitPrice
            {
                get => _unitPrice;
                set => SetProperty(ref _unitPrice, value);
            }
        }
    }
}
