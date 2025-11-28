using Microsoft.Data.SqlClient;
using System;
using Undy.ViewModels.Helpers;
using System.Windows.Input;
using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels
{
    public class TestPurchaseOrderViewModel : BaseViewModel
    {
        private readonly IBaseRepository<TestPurchaseOrder, Guid> _testPurchaseOrderRepo;
        private readonly IBaseRepository<Product, Guid> _productRepo;
        public TestPurchaseOrderViewModel(IBaseRepository<TestPurchaseOrder, Guid> testPurchaseOrderRepo, IBaseRepository<Product, Guid> productRepo)
        {
            _testPurchaseOrderRepo = testPurchaseOrderRepo;
            _productRepo = productRepo;
        }
    }
}
