using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels
{
    public class PaymentViewModel : BaseViewModel
    {
        private IBaseRepository<SalesOrder> _salesOrderRepo;

        public PaymentViewModel(IBaseRepository<SalesOrder> salesOrderRepo)
        {
            _salesOrderRepo = salesOrderRepo;
        }
    }
}
