namespace Undy.Features.ViewModel {
    public class ProductViewModel : BaseViewModel {
        private readonly IBaseRepository<Product, Guid> _productRepo;
        private readonly ICollectionView _productView;
        public ObservableCollection<Product> Products => _productRepo.Items;
        public ICollectionView ProductView => _productView;

        public ProductViewModel(IBaseRepository<Product, Guid> productRepo) {
            _productRepo = productRepo;

            _productView = CollectionViewSource.GetDefaultView(Products);
            _productView.SortDescriptions.Add(new SortDescription(nameof(Product.NumberInStock), ListSortDirection.Descending));
        }
    }
}
