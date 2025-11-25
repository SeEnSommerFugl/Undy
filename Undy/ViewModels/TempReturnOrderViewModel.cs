using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels
{
    public class TestReturnOrderViewModel : BaseViewModel
    {
        private IBaseRepository<TestReturnOrder, Guid> _tempReturnRepo;

        public TestReturnOrderViewModel(IBaseRepository<TestReturnOrder, Guid> tempReturnRepo)
        {
            _tempReturnRepo = tempReturnRepo;
        }
    }
}

