namespace Undy.Features.Helpers
{
    public class SalesOrderService
    {
        private readonly SalesOrderDBRepository _salesOrderRepo;
        private readonly ProductSalesOrderDBRepository _productSalesOrderRepo;

        public SalesOrderService()
        {
            _salesOrderRepo = new SalesOrderDBRepository();
            _productSalesOrderRepo = new ProductSalesOrderDBRepository();
        }

        public async Task CreateSalesOrderWithProducts(SalesOrder salesOrder, List<ProductSalesOrder> productSalesOrderLines)
        {
            using var con = await DB.OpenConnection();
            using var transaction = con.BeginTransaction();

            try
            {
                await Task.WhenAll(
                _salesOrderRepo.AddRangeAsync(new[] { salesOrder }, con, transaction),
                Task.CompletedTask
                );

                await _productSalesOrderRepo.AddRangeAsync(productSalesOrderLines, con, transaction);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
