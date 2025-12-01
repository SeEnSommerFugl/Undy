using Microsoft.Data.SqlClient;
using System;
using Undy.ViewModels.Helpers;
using System.Windows.Input;
using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels
{
    public class TestWholesaleOrderViewModel : BaseViewModel
    {
        private readonly IBaseRepository<TestWholesaleOrder, Guid> _testPurchaseOrderRepo;
        private readonly IBaseRepository<Product, Guid> _productRepo;
        public TestWholesaleOrderViewModel(IBaseRepository<TestWholesaleOrder, Guid> testPurchaseOrderRepo, IBaseRepository<Product, Guid> productRepo)
        {
            _testPurchaseOrderRepo = testPurchaseOrderRepo;
            _productRepo = productRepo;
        }
    }
}
