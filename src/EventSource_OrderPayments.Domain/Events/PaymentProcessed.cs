using EventSource_OrderPayments.Domain.ValueObjects;

namespace EventSource_OrderPayments.Domain.Events
{
    public class PaymentProcessed : BaseEvent
    {
        public Guid PaymentId { get; }
        public Guid OrderId { get; }
        public Money Amount { get; }
        public string PaymentMethod { get; }
        public bool IsSuccessful { get; }

        public PaymentProcessed(Guid paymentId, Guid orderId, Money amount, string paymentMethod, bool isSuccessful, int version)
            : base(paymentId, version)
        {
            PaymentId = paymentId;
            OrderId = orderId;
            Amount = amount;
            PaymentMethod = paymentMethod;
            IsSuccessful = isSuccessful;
        }
    }
}