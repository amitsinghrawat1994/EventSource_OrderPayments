namespace EventSource_OrderPayments.Domain.Aggregates
{
    public abstract class AggregateBase
    {
        public int Version { get; protected set; } = 0;
    }
}