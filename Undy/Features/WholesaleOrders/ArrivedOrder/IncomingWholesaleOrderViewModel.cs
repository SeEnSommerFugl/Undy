using System.Collections.ObjectModel;
using Undy.Data.Repository;
using Undy.Models;

namespace Undy.Features.ViewModel
{
    public class IncomingWholesaleOrderViewModel : BaseViewModel
    {
        private readonly IBaseRepository<WholesaleOrder, Guid> _wholesaleOrderRepo;
        private readonly IBaseRepository<Product, Guid> _productRepo;
        private readonly WholesaleOrderLineDBRepository _wholesaleOrderLineRepo;

        private WholesaleOrder? _selectedWholesaleOrder;

        public IncomingWholesaleOrderViewModel(
            IBaseRepository<WholesaleOrder, Guid> wholesaleOrderRepo,
            IBaseRepository<Product, Guid> productRepo,
            WholesaleOrderLineDBRepository wholesaleOrderLineRepo)
        {
            _wholesaleOrderRepo = wholesaleOrderRepo;
            _productRepo = productRepo;
            _wholesaleOrderLineRepo = wholesaleOrderLineRepo;

            SelectedOrderLines = new ObservableCollection<WholesaleOrderLine>();
        }

        public ObservableCollection<WholesaleOrder> WholesaleOrders => _wholesaleOrderRepo.Items;

        public WholesaleOrder? SelectedWholesaleOrder
        {
            get => _selectedWholesaleOrder;
            set
            {
                if (_selectedWholesaleOrder == value) return;
                _selectedWholesaleOrder = value;
                OnPropertyChanged();

                _ = LoadSelectedOrderLinesAsync();
            }
        }

        public ObservableCollection<WholesaleOrderLine> SelectedOrderLines { get; }

        private async Task LoadSelectedOrderLinesAsync()
        {
            SelectedOrderLines.Clear();

            if (SelectedWholesaleOrder is null)
                return;

            var lines = await _wholesaleOrderLineRepo.GetByWholesaleOrderIdAsync(SelectedWholesaleOrder.WholesaleOrderID);
            foreach (var line in lines)
            {
                SelectedOrderLines.Add(line);
            }
        }
    }
}
