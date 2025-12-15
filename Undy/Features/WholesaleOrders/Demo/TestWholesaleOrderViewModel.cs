using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Undy.Models;

namespace Undy.Features.ViewModel
{
    public class TestWholesaleOrderViewModel : BaseViewModel
    {
        private readonly IBaseRepository<WholesaleOrder, Guid> _wholesaleOrderRepo;
        private readonly IBaseRepository<Product, Guid> _productRepo;
        private readonly IBaseRepository<ProductWholesaleOrder, Guid> _productWholesaleOrderRepo;

        public ObservableCollection<Product> Products => _productRepo.Items;

        // UI-linjer (samler info fra Product + linjedata)
        public ObservableCollection<WholesaleOrderDisplay> WholesaleOrderLines { get; } = new();

        public ICommand ConfirmCommand { get; }
        public ICommand RemoveWholesaleOrderLineCommand { get; }
        public ICommand AddProductCommand { get; }

        private Product _selectedProduct;
        public Product SelectedProduct
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

        public TestWholesaleOrderViewModel(
            IBaseRepository<WholesaleOrder, Guid> wholesaleOrderRepo,
            IBaseRepository<Product, Guid> productRepo,
            IBaseRepository<ProductWholesaleOrder, Guid> productWholesaleOrderRepo)
        {
            _wholesaleOrderRepo = wholesaleOrderRepo;
            _productRepo = productRepo;
            _productWholesaleOrderRepo = productWholesaleOrderRepo;

            ConfirmCommand = new RelayCommand(async _ => await CreateWholesaleOrderAsync());
            AddProductCommand = new RelayCommand(_ => AddProduct());
            RemoveWholesaleOrderLineCommand = new RelayCommand(line => RemoveWholesaleOrderLine(line as WholesaleOrderDisplay));

            // Valgfrit: så feltet ikke starter tomt
            ExpectedDeliveryDate ??= DateTime.Today;
        }

        private void AddProduct()
        {
            if (SelectedProduct == null || Quantity <= 0)
                return;

            // (Valgfrit) Hvis du vil undgå dubletter: merge samme ProductID
            var existing = WholesaleOrderLines.FirstOrDefault(l => l.ProductID == SelectedProduct.ProductID);
            if (existing != null)
            {
                // OBS: WholesaleOrderDisplay properties er set; så denne virker kun hvis de ikke er read-only
                existing.Quantity += Quantity;
                OnPropertyChanged(nameof(WholesaleOrderLines));
                return;
            }

            var line = new WholesaleOrderDisplay
            {
                // Product
                ProductID = SelectedProduct.ProductID,
                ProductNumber = SelectedProduct.ProductNumber,
                ProductName = SelectedProduct.ProductName,
                Size = SelectedProduct.Size,
                Colour = SelectedProduct.Colour,

                // ProductWholesaleOrder-linje
                Quantity = Quantity,
                UnitPrice = SelectedProduct.Price,   // <-- ret hvis pris hedder noget andet
                QuantityReceived = 0
            };

            WholesaleOrderLines.Add(line);
        }

        private void RemoveWholesaleOrderLine(WholesaleOrderDisplay line)
        {
            if (line != null)
                WholesaleOrderLines.Remove(line);
        }

        private async Task CreateWholesaleOrderAsync()
        {
            if (WholesaleOrderLines.Count == 0 || ExpectedDeliveryDate == null)
                return;

            var newWholesaleOrderId = Guid.NewGuid();

            // Ordrehoved (kun det nødvendige)
            var wholesaleOrder = new WholesaleOrder
            {
                WholesaleOrderID = newWholesaleOrderId,
                WholesaleOrderDate = DateOnly.FromDateTime(DateTime.Today),
                ExpectedDeliveryDate = DateOnly.FromDateTime(ExpectedDeliveryDate.Value)
            };

            await _wholesaleOrderRepo.AddAsync(wholesaleOrder);

            // Ordrelinjer (koblingstabel) - ProductID er korrekt i dit projekt
            var lines = WholesaleOrderLines.Select(l => new ProductWholesaleOrder
            {
                WholesaleOrderID = newWholesaleOrderId,
                ProductID = l.ProductID,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice,
                QuantityReceived = 0
            });

            await _productWholesaleOrderRepo.AddRangeAsync(lines);
        }
    }
}
