namespace EventSource_OrderPayments.Domain.Events
{
    public interface IEvent
    {
        Guid AggregateId { get; }
        int Version { get; }
    }
}