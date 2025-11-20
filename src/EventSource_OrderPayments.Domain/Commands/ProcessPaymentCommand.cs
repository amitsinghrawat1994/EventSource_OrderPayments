using EventSource_OrderPayments.Domain.ValueObjects;

namespace EventSource_OrderPayments.Domain.Commands
{
    public class ProcessPaymentCommand
    {
        public Guid AggregateId { get; }
        public Guid OrderId { get; }
        public Money Amount { get; }
        public string PaymentMethod { get; }

        public ProcessPaymentCommand(Guid aggregateId, Guid orderId, Money amount, string paymentMethod)
        {
            AggregateId = aggregateId;
            OrderId = orderId;
            Amount = amount;
            PaymentMethod = paymentMethod;
        }
    }
}