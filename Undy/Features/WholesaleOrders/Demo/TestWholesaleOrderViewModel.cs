using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Undy.Data.Repository;
using Undy.Models;

namespace Undy.Features.ViewModel
{
    public class TestWholesaleOrderViewModel : BaseViewModel
    {
        private readonly IBaseRepository<WholesaleOrder, Guid> _wholesaleOrderRepo;
        private readonly IBaseRepository<Product, Guid> _productRepo;
        private readonly WholesaleOrderLineDBRepository _wholesaleOrderLineRepo;

        public ObservableCollection<Product> Products => _productRepo.Items;

        // Bound in XAML -> must be public and the item type must be at least as accessible.
        public ObservableCollection<TestWholesaleOrderLineEntryViewModel> WholesaleOrderLines { get; } = new();

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

        private DateTime? _expectedDeliveryDate;
        public DateTime? ExpectedDeliveryDate
        {
            get => _expectedDeliveryDate;
            set => SetProperty(ref _expectedDeliveryDate, value);
        }

        public ICommand ConfirmCommand { get; }
        public ICommand AddProductCommand { get; }
        public ICommand RemoveWholesaleOrderLineCommand { get; }

        public TestWholesaleOrderViewModel(
            IBaseRepository<WholesaleOrder, Guid> wholesaleOrderRepo,
            IBaseRepository<Product, Guid> productRepo,
            WholesaleOrderLineDBRepository wholesaleOrderLineRepo)
        {
            _wholesaleOrderRepo = wholesaleOrderRepo;
            _productRepo = productRepo;
            _wholesaleOrderLineRepo = wholesaleOrderLineRepo;

            ConfirmCommand = new RelayCommand(async _ => await CreateWholesaleOrderAsync());
            AddProductCommand = new RelayCommand(_ => AddProduct(), _ => CanAddProduct());
            RemoveWholesaleOrderLineCommand = new RelayCommand(line =>
                RemoveWholesaleOrderLine(line as TestWholesaleOrderLineEntryViewModel));
        }

        private bool CanAddProduct()
        {
            return SelectedProduct != null && Quantity > 0;
        }

        private void AddProduct()
        {
            if (SelectedProduct == null)
                return;

            var existing = WholesaleOrderLines.FirstOrDefault(x => x.ProductID == SelectedProduct.ProductID);
            if (existing != null)
            {
                existing.Quantity += Quantity;
                return;
            }

            WholesaleOrderLines.Add(new TestWholesaleOrderLineEntryViewModel
            {
                ProductID = SelectedProduct.ProductID,
                ProductName = SelectedProduct.ProductName,
                UnitPrice = SelectedProduct.Price,
                Quantity = Quantity,
                QuantityReceived = 0
            });

            Quantity = 0;
        }

        private void RemoveWholesaleOrderLine(TestWholesaleOrderLineEntryViewModel? line)
        {
            if (line == null) return;
            WholesaleOrderLines.Remove(line);
        }

        private async Task CreateWholesaleOrderAsync()
        {
            var newOrder = new WholesaleOrder
            {
                WholesaleOrderID = Guid.NewGuid(),
                WholesaleOrderDate = DateOnly.FromDateTime(DateTime.Now),
                ExpectedDeliveryDate = ExpectedDeliveryDate.HasValue
                    ? DateOnly.FromDateTime(ExpectedDeliveryDate.Value)
                    : DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
                OrderStatus = "Pending"
            };

            await _wholesaleOrderRepo.AddAsync(newOrder);

            var lines = WholesaleOrderLines.Select(l => new WholesaleOrderLine
            {
                WholesaleOrderID = newOrder.WholesaleOrderID,
                ProductID = l.ProductID,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice,
                QuantityReceived = l.QuantityReceived
            }).ToList();

            await _wholesaleOrderLineRepo.AddRangeAsync(lines);

            WholesaleOrderLines.Clear();
            SelectedProduct = null;
            Quantity = 0;
            ExpectedDeliveryDate = null;
        }

        // Public because WholesaleOrderLines is public and bound in XAML.
        public class TestWholesaleOrderLineEntryViewModel : BaseViewModel
        {
            private Guid _productId;
            public Guid ProductID
            {
                get => _productId;
                set => SetProperty(ref _productId, value);
            }

            private string _productName = string.Empty;
            public string ProductName
            {
                get => _productName;
                set => SetProperty(ref _productName, value);
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

            private int _quantityReceived;
            public int QuantityReceived
            {
                get => _quantityReceived;
                set => SetProperty(ref _quantityReceived, value);
            }
        }
    }
}
