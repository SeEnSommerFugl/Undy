using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using System.Data;
using Undy.Data;
using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels
{
    public class IncomingWholeSaleOrderViewModel : BaseViewModel
    {
        private readonly IBaseRepository<WholesaleOrder, Guid> _wholesaleOrderRepo;
        private readonly IBaseRepository<ProductWholesaleOrder, Guid> _productWholesaleOrderRepo;

        private WholesaleOrder _SelectedOrder;
        private bool _IsFullyReceived;
        private string _statusMessage;


        public IncomingWholeSaleOrderViewModel(IBaseRepository<WholesaleOrder, Guid> purchaseOrderRepo, IBaseRepository<Product, Guid> productRepo, IBaseRepository<ProductWholesaleOrder, Guid> productWholesaleOrderRepo)
        {
            _wholesaleOrderRepo = purchaseOrderRepo;            
            _productWholesaleOrderRepo = productWholesaleOrderRepo;

        }

        public ObservableCollection<WholesaleOrder> WholesaleOrders
            => _wholesaleOrderRepo.Items;

        private WholesaleOrder? _selectedOrder;
        public WholesaleOrder? SelectedOrder
        {
            get => _selectedOrder;
            set => SetProperty(ref _selectedOrder, value);
        }
        public ObservableCollection<IncomingOrderLineViewModel> Lines { get; }

        private bool _isFullyReceived;
        public bool IsFullyReceived
        {
            get => _isFullyReceived;
            set => SetProperty(ref _isFullyReceived, value);
        }
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        //public async Task LoadAsync()
        
        
        //public async Task ConfirmAsync()
        
     
        
    }
}
