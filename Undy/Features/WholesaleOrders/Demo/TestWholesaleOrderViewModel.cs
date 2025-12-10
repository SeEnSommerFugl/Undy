namespace Undy.Features.WholesaleOrders.Demo
{
    public class TestWholesaleOrderViewModel : BaseViewModel
    {
        private readonly IBaseRepository<WholesaleOrder, Guid> _wholesaleOrderRepo;
        private readonly IBaseRepository<Product, Guid> _productRepo;
        public TestWholesaleOrderViewModel(IBaseRepository<WholesaleOrder, Guid> wholesaleOrderRepo, IBaseRepository<Product, Guid> productRepo)
        {
            _wholesaleOrderRepo = wholesaleOrderRepo;
            _productRepo = productRepo;
        }
    }
}
