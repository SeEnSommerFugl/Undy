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
        private readonly IBaseRepository<Product, Guid> _repo;

        private readonly ICollectionView _productView;
        public ICollectionView ProductView => _productView;

        public System.Collections.ObjectModel.ObservableCollection<Product> Products => _repo.Items;

        public ICommand OpenAddCommand { get; }
        public ICommand OpenEditCommand { get; }

        public ProductViewModel(IBaseRepository<Product, Guid> repo)
        {
            _repo = repo;

            _productView = CollectionViewSource.GetDefaultView(Products);
            _productView.SortDescriptions.Add(new SortDescription(nameof(Product.ProductNumber), ListSortDirection.Ascending));

            OpenAddCommand = new RelayCommand(_ => _ = OpenProductDialogAsync(ProductDialogMode.Add, null));
            OpenEditCommand = new RelayCommand(p =>
            {
                if (p is Product prod)
                    _ = OpenProductDialogAsync(ProductDialogMode.Edit, prod);
            });
        }

        public async Task InitializeAsync()
        {
            await _repo.InitializeAsync();
        }

        private async Task OpenProductDialogAsync(ProductDialogMode mode, Product? product)
        {
            if (_repo is not ProductDBRepository productRepo)
            {
                MessageBox.Show("Product repository er ikke ProductDBRepo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dlg = new ProductDialog(
                mode,
                product,
                addAsync: async created => await productRepo.AddAsync(created),
                editAsync: async updated => await productRepo.UpdateAsync(updated))

            {
                Owner = Application.Current?.MainWindow
            };

            dlg.ShowDialog();
            await Task.CompletedTask;
        }
    }
}
