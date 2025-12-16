using System.Globalization;
using Undy.Data.Repository;
using Undy.Features.Helpers;

namespace Undy.Features.ViewModel
{
    /// <summary>
    /// ViewModel for StartPage KPI cards.
    /// Holds raw numeric KPI values. Formatting is done in XAML.
    /// </summary>
    public class StartPageViewModel : BaseViewModel
    {
        private readonly StartPageDBRepository _repo;

        public int PackedToday { get; private set; }
        public int ReadyToPick { get; private set; }
        public int PackedTotal { get; private set; }

        // Keep BOTH:
        // - AverageCustomerValue = average value per customer
        // - AverageOrderValue (AOV) = average value per order
        public decimal AverageCustomerValue { get; private set; }
        public decimal AverageOrderValue { get; private set; }

        public int WholesaleOnTheWay { get; private set; }

        // Stored procedure may currently return a formatted string (e.g. "2,86 %").
        // We store it as decimal fraction (0.0286m) so XAML can format as percent.
        public decimal TotalReturnRate { get; private set; }

        public int OutstandingPayments { get; private set; }
        public int UniqueCustomers { get; private set; }

        public RelayCommand RefreshCommand { get; }

        public StartPageViewModel(StartPageDBRepository repo)
        {
        provisioning: _repo = repo;

            RefreshCommand = new RelayCommand(
                _ => _ = RefreshAsync(),
                _ => !IsBusy
            );

            _ = RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                var today = DateOnly.FromDateTime(DateTime.Today);

                var packedTodayTask = _repo.GetPackedTodayAsync(today);
                var readyToPickTask = _repo.GetReadyToPickAsync();
                var packedTotalTask = _repo.GetPackedTotalAsync();

                // Existing: may be string (depends on your repo). We parse safely below.
                var averageCustomerValueTask = _repo.GetAverageCustomerValueAsync();

                // New KPI (AOV). Expected to be decimal-returning repo method.
                var averageOrderValueTask = _repo.GetAverageOrderValueAsync();

                var wholesaleOnTheWayTask = _repo.GetWholesaleOnTheWayAsync();

                // Existing: repo previously returned pre-formatted string ("2,86 %").
                var totalReturnRateTask = _repo.GetTotalReturnRateAsync();

                var outstandingPaymentsTask = _repo.GetOutstandingPaymentsAsync();
                var uniqueCustomersTask = _repo.GetUniqueCustomersAsync();

                await Task.WhenAll(
                    packedTodayTask,
                    readyToPickTask,
                    packedTotalTask,
                    averageCustomerValueTask,
                    averageOrderValueTask,
                    wholesaleOnTheWayTask,
                    totalReturnRateTask,
                    outstandingPaymentsTask,
                    uniqueCustomersTask
                );

                PackedToday = packedTodayTask.Result;
                ReadyToPick = readyToPickTask.Result;
                PackedTotal = packedTotalTask.Result;

                AverageCustomerValue = ParseDecimal(averageCustomerValueTask.Result);
                AverageOrderValue = averageOrderValueTask.Result;

                WholesaleOnTheWay = wholesaleOnTheWayTask.Result;

                TotalReturnRate = ParsePercentToFraction(totalReturnRateTask.Result);

                OutstandingPayments = outstandingPaymentsTask.Result;
                UniqueCustomers = uniqueCustomersTask.Result;

                OnPropertyChanged(string.Empty); // refresh all bindings
            }
            catch
            {
                PackedToday = 0;
                ReadyToPick = 0;
                PackedTotal = 0;

                AverageCustomerValue = 0m;
                AverageOrderValue = 0m;

                WholesaleOnTheWay = 0;
                TotalReturnRate = 0m;

                OutstandingPayments = 0;
                UniqueCustomers = 0;

                OnPropertyChanged(string.Empty);
            }
            finally
            {
                IsBusy = false;
                RefreshCommand.RaiseCanExecuteChanged();
            }
        }

        private static decimal ParseDecimal(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0m;

            // Accept both da-DK and invariant formatted decimals.
            var dk = CultureInfo.GetCultureInfo("da-DK");
            if (decimal.TryParse(input, NumberStyles.Number, dk, out var dkVal))
                return dkVal;

            if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out var invVal))
                return invVal;

            return 0m;
        }

        private static decimal ParsePercentToFraction(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0m;

            // Typical format from repo/proc: "2,86 %" or "2.86%" etc.
            var cleaned = input.Replace("%", string.Empty).Trim();

            var dk = CultureInfo.GetCultureInfo("da-DK");
            if (decimal.TryParse(cleaned, NumberStyles.Number, dk, out var dkPercent))
                return dkPercent / 100m;

            if (decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.InvariantCulture, out var invPercent))
                return invPercent / 100m;

            return 0m;
        }
    }
}
