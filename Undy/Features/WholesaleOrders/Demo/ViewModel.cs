using Microsoft.Data.SqlClient;
using System;
using Undy.Features.Helpers;
using System.Windows.Input;
using Undy.Data.Repository;
using Undy.Models;
using Undy.Features.Base;

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
