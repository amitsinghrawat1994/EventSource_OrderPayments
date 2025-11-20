using EventSource_OrderPayments.Domain.ValueObjects;

namespace EventSource_OrderPayments.Domain.Events
{
    public class OrderCreated : BaseEvent
    {
        public Guid OrderId { get; }
        public Guid CustomerId { get; }
        public List<OrderItem> Items { get; }
        public Money TotalAmount { get; }

        public OrderCreated(Guid orderId, Guid customerId, List<OrderItem> items, Money totalAmount, int version)
            : base(orderId, version)
        {
            OrderId = orderId;
            CustomerId = customerId;
            Items = items;
            TotalAmount = totalAmount;
        }
    }
}