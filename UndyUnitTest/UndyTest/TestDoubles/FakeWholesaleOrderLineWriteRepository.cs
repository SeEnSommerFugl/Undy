using Undy.Data.Repository;
using Undy.Models;

namespace UndyTest.TestDoubles;

internal sealed class FakeWholesaleOrderLineWriteRepository : WholesaleOrderLineDBRepository
{
    public List<WholesaleOrderLine> AddedLines { get; } = new();

    public override Task AddRangeAsync(IEnumerable<WholesaleOrderLine> entities)
    {
        AddedLines.Clear();
        AddedLines.AddRange(entities);
        return Task.CompletedTask;
    }
}
