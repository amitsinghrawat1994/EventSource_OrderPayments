using EventSource_OrderPayments.Domain.Events;
using EventSource_OrderPayments.Domain.Commands;
using EventSource_OrderPayments.Domain.ValueObjects;

namespace EventSource_OrderPayments.Domain.Aggregates
{
    public class OrderAggregate : AggregateBase
    {
        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public List<OrderItem> Items { get; private set; } = new();
        public Money TotalAmount { get; private set; }
        public string Status { get; private set; } = "Pending";

        public void Apply(OrderCreated @event)
        {
            Id = @event.OrderId;
            CustomerId = @event.CustomerId;
            Items = @event.Items;
            TotalAmount = @event.TotalAmount;
            Status = "Created";
        }

        public IEnumerable<IEvent> CreateOrder(CreateOrderCommand command)
        {
            if (Items.Any()) throw new InvalidOperationException("Order already created");
            var totalAmount = new Money(command.Items.Sum(i => i.TotalPrice.Amount), "USD");
            yield return new OrderCreated(command.AggregateId, command.CustomerId, command.Items, totalAmount, Version + 1);
        }
    }
}