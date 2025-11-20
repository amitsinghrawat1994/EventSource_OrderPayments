using EventSource_OrderPayments.Domain.Events;

namespace EventSource_OrderPayments.Infrastructure.Persistence
{
    public interface IEventStore
    {
        Task AppendEventsAsync(Guid aggregateId, IEnumerable<IEvent> events, CancellationToken cancellationToken = default);
        Task<IEnumerable<IEvent>> GetEventsAsync(Guid aggregateId, CancellationToken cancellationToken = default);
    }
}