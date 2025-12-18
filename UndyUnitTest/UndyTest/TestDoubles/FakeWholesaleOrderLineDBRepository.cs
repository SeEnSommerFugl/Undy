using Undy.Data.Repository;
using Undy.Models;

namespace UndyTest.TestDoubles;

internal sealed class FakeWholesaleOrderLineDBRepository : WholesaleOrderLineDBRepository
{
    public List<(Guid WholesaleOrderID, Guid ProductID, int ReceiveQuantity)> LastReceipts { get; } = new();

    public Func<Guid, List<WholesaleOrderLine>> LinesByOrderId { get; set; } = _ => new();

    public override Task<List<WholesaleOrderLine>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        var list = new List<WholesaleOrderLine>();
        foreach (var id in ids)
            list.AddRange(LinesByOrderId(id));
        return Task.FromResult(list);
    }

    public override Task ProcessReceiptLinesAsync(IEnumerable<(Guid WholesaleOrderID, Guid ProductID, int ReceiveQuantity)> receipts)
    {
        LastReceipts.Clear();
        LastReceipts.AddRange(receipts);
        return Task.CompletedTask;
    }
}
