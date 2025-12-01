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
        private readonly IBaseRepository<TestWholesaleOrder, Guid> _testWholesaleOrderRepo;
        private readonly IBaseRepository<Product, Guid> _productRepo;
        public TestWholesaleOrderViewModel(IBaseRepository<TestWholesaleOrder, Guid> testWholesaleOrderRepo, IBaseRepository<Product, Guid> productRepo)
        {
            _testWholesaleOrderRepo = testWholesaleOrderRepo;
            _productRepo = productRepo;
        }
    }
}
