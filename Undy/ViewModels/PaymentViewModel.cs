using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels
{
    public class PaymentViewModel : BaseViewModel
    {
        private IBaseRepository<SalesOrder, Guid> _salesOrderRepo;

        public PaymentViewModel(IBaseRepository<SalesOrder, Guid> salesOrderRepo)
        {
            _salesOrderRepo = salesOrderRepo;
        }
    }
}
