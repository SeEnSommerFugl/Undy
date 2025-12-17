namespace Undy.Features.SalesOrders
{
    public sealed class SalesOrderDetailRow
    {
        public string Label { get; }
        public object? Value { get; }

        public SalesOrderDetailRow(string label, object? value)
        {
            Label = label;
            Value = value;
        }
    }
}
