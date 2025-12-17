using Undy.Features.Products.AddProduct;

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

        public ProductViewModel(
            IBaseRepository<Product, Guid> productRepo,
            IAddProductDialogService addProductDialogService)
        {
            _productRepo = productRepo;
            _addProductDialogService = addProductDialogService;

            _productView = CollectionViewSource.GetDefaultView(Products);
            _productView.SortDescriptions.Add(
                new SortDescription(nameof(Product.ProductNumber), ListSortDirection.Ascending));

            OpenAddProductDialogCommand = new RelayCommand(_ =>
            {
                _ = OpenAddProductDialogAsync();
            });

            OpenEditProductDialogCommand = new RelayCommand(p =>
            {
                if (p is not Product prod) return;
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
            catch
            {
                await _productRepo.InitializeAsync();
                throw;
            }
        }

        private async Task OpenEditProductDialogAsync(Product product)
        {
            var dialog = new Views.EditProductDialog(product)
            {
                Owner = Application.Current?.MainWindow
            };

            var ok = dialog.ShowDialog();
            if (ok != true) return;

            var updated = dialog.UpdatedProduct;
            if (updated is null) return;

            try
            {
                await _productRepo.UpdateAsync(updated);
            }
            catch
            {
                await _productRepo.InitializeAsync();
                throw;
            }
        }
    }
}
