using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Undy.Data.Repository;
using Undy.Features.Helpers;
using Undy.Features.Products.AddProduct;
using Undy.Models;

namespace Undy.Features.ViewModel
{
    public sealed class ProductViewModel : BaseViewModel
    {
        private readonly IBaseRepository<Product, Guid> _productRepo;
        private readonly IAddProductDialogService _addProductDialogService;

        private readonly ICollectionView _productView;
        public ICollectionView ProductView => _productView;

        public System.Collections.ObjectModel.ObservableCollection<Product> Products => _productRepo.Items;

        public ICommand OpenAddProductDialogCommand { get; }
        public ICommand OpenEditProductDialogCommand { get; }

        public ProductViewModel(IBaseRepository<Product, Guid> productRepo)
            : this(productRepo, new AddProductDialogService())
        {
        }

        public ProductViewModel(IBaseRepository<Product, Guid> productRepo, IAddProductDialogService addProductDialogService)
        {
            _productRepo = productRepo;
            _addProductDialogService = addProductDialogService;

            _productView = CollectionViewSource.GetDefaultView(Products);
            _productView.SortDescriptions.Add(new SortDescription(nameof(Product.ProductNumber), ListSortDirection.Ascending));

            OpenAddProductDialogCommand = new RelayCommand(_ => _ = OpenAddProductDialogAsync());
            OpenEditProductDialogCommand = new RelayCommand(p =>
            {
                if (p is Product prod)
                    _ = OpenEditProductDialogAsync(prod);
            });
        }

        public async Task InitializeAsync()
        {
            await _productRepo.InitializeAsync();
        }

        private async Task OpenAddProductDialogAsync()
        {
            var created = _addProductDialogService.ShowDialog(Application.Current?.MainWindow);
            if (created is null) return;

            try
            {
                await _productRepo.AddAsync(created);
            }
            catch (Exception ex)
            {
                await _productRepo.InitializeAsync();
                MessageBox.Show(ex.Message, "Insert failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task OpenEditProductDialogAsync(Product product)
        {
            var dialog = new EditProductDialog(product)
            {
                Owner = Application.Current?.MainWindow
            };

            var ok = dialog.ShowDialog();
            if (ok != true) return;

            var updated = dialog.UpdatedProduct;
            if (updated is null) return;

            try
            {
                // update for SQL contract (lookup by old ProductNumber + optional NewProductNumber)
                if (_productRepo is ProductDBRepository productRepo)
                {
                    await productRepo.UpdateByProductNumberAsync(dialog.OriginalProductNumber, updated);
                }
                else
                {
                    // fallback
                    await _productRepo.UpdateAsync(updated);
                    await _productRepo.InitializeAsync();
                }
            }
            catch (Exception ex)
            {
                await _productRepo.InitializeAsync();
                MessageBox.Show(ex.Message, "Update failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
