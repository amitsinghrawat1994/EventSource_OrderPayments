namespace EventSource_OrderPayments.Domain.Events
{
    public abstract class BaseEvent : IEvent
    {
        public Guid AggregateId { get; }
        public int Version { get; }

        protected BaseEvent(Guid aggregateId, int version)
        {
            AggregateId = aggregateId;
            Version = version;
        }
    }
}