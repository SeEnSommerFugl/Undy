// Fil: Undy/Undy/Features/Products/ProductViewModel.cs

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Undy.Data.Repository;
using Undy.Features.Helpers;
using Undy.Models;

namespace Undy.Features.ViewModel
{
    public sealed class ProductViewModel : BaseViewModel
    {
        private readonly IBaseRepository<Product, Guid> _productRepo;

        // DataGrid binder typisk til dette via CollectionView (sort/filter-friendly)
        private readonly ICollectionView _productView;
        public ICollectionView ProductView => _productView;

        // Underliggende collection (kommer fra dit BaseDBRepository-pattern)
        public System.Collections.ObjectModel.ObservableCollection<Product> Products => _productRepo.Items;

        private Product? _editingProduct;
        public Product? EditingProduct
        {
            get => _editingProduct;
            private set => SetProperty(ref _editingProduct, value);
        }

        public ICommand BeginEditQuantityCommand { get; }
        public ICommand CommitEditedQuantityCommand { get; }

        public ProductViewModel(IBaseRepository<Product, Guid> productRepo)
        {
            _productRepo = productRepo;

            _productView = CollectionViewSource.GetDefaultView(Products);
            _productView.SortDescriptions.Add(
                new SortDescription(nameof(Product.ProductNumber), ListSortDirection.Ascending));

            BeginEditQuantityCommand = new RelayCommand(p =>
            {
                if (p is Product prod)
                    EditingProduct = prod;
            });

            CommitEditedQuantityCommand = new RelayCommand(_ =>
            {
                _ = CommitEditedQuantityAsync();
            });
        }

        // Kald denne når VM oprettes (fx fra App.xaml.cs composition)
        public async Task InitializeAsync()
        {
            await _productRepo.InitializeAsync();
        }

        private async Task CommitEditedQuantityAsync()
        {
            if (EditingProduct is null) return;

            // Simple validation: negative stock not allowed
            if (EditingProduct.NumberInStock < 0)
            {
                await _productRepo.InitializeAsync(); // revert to DB state
                return;
            }

            try
            {
                // Opdaterer hele product-row via eksisterende usp_Update_Product / repo
                await _productRepo.UpdateAsync(EditingProduct);
            }
            catch
            {
                // Hvis DB-update fejler, reload fra DB så UI ikke står med forkert tal
                await _productRepo.InitializeAsync();
                throw;
            }
        }
    }
}
