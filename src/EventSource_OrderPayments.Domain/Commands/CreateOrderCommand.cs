using EventSource_OrderPayments.Domain.ValueObjects;

namespace EventSource_OrderPayments.Domain.Commands
{
    public class CreateOrderCommand //: BaseCommand
    {
        public Guid AggregatedId { get; }
        public Guid CustomerId { get; }
        public Guid AggregateId { get; }  // Ensure this is 'AggregateId'
        public List<OrderItem> Items { get; }

        public CreateOrderCommand(Guid aggregatedId, Guid customerId, List<OrderItem> items)
        {
            AggregatedId = aggregatedId;
            CustomerId = customerId;
            Items = items;
        }
    }
}