using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Undy.Views
{
    public class ProductViewModel : BaseViewModel
    {
        private readonly IProductRepository _productRepo;

        public ObservableCollection<Product> Products { get; } = new();

        private Product? _editingProduct;
        public Product? EditingProduct
        {
            get => _editingProduct;
            private set => SetField(ref _editingProduct, value);
        }

        public ICommand BeginEditQuantityCommand { get; }
        public ICommand CommitEditedQuantityCommand { get; }
        public ICommand OpenAddProductDialogCommand { get; }

        public ProductViewModel(IProductRepository productRepo)
        {
            _productRepo = productRepo;

            BeginEditQuantityCommand = new RelayCommand<Product>(BeginEditQuantity);
            CommitEditedQuantityCommand = new RelayCommand(async () => await CommitEditedQuantityAsync());

            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            Products.Clear();
            var items = await _productRepo.GetAllAsync();
            foreach (var p in items)
                Products.Add(p);
        }

        private void BeginEditQuantity(Product? product)
        {
            if (product is null) return;
            EditingProduct = product;

            // View handles focus/start-edit; VM only tracks which row is being edited.
            // (We keep this simple and robust)
        }

        private async Task CommitEditedQuantityAsync()
        {
            if (EditingProduct is null) return;

            // Validation: must be >= 0 (adjust if your domain allows negative)
            if (EditingProduct.NumberInStock < 0)
            {
                // revert by reload (simple + safe)
                await LoadAsync();
                return;
            }

            await _productRepo.UpdateStockAsync(EditingProduct.ProductID, EditingProduct.NumberInStock);
        }

        private void OpenAddProductDialog()
        {
            var vm = new AddProductDialogViewModel(async created =>
            {
                await _productRepo.InsertAsync(created);
                Products.Add(created);
            });

            var dialog = new AddProductDialog
            {
                DataContext = vm
            };

            dialog.ShowDialog();
        }
    }
}
