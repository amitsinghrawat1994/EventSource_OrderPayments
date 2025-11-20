using EventSource_OrderPayments.Domain.Commands;
using EventSource_OrderPayments.Domain.Events;
using EventSource_OrderPayments.Domain.ValueObjects;
using Marten.Events.Aggregation;

namespace EventSource_OrderPayments.Domain.Aggregates
{
    public class PaymentAggregate : AggregateBase
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public Money Amount { get; private set; }
        public string PaymentMethod { get; private set; }
        public bool IsSuccessful { get; private set; }
        public string Status { get; private set; } = "Pending";

        public void Apply(PaymentProcessed @event)
        {
            Id = @event.PaymentId;
            OrderId = @event.OrderId;
            Amount = @event.Amount;
            PaymentMethod = @event.PaymentMethod;
            IsSuccessful = @event.IsSuccessful;
            Status = @event.IsSuccessful ? "Completed" : "Failed";
        }

        public IEnumerable<IEvent> ProcessPayment(ProcessPaymentCommand command)
        {
            if (Status != "Pending") throw new InvalidOperationException("Payment already processed");

            // Simulate payment processing logic
            var isSuccessful = true; // In real scenario, integrate with payment gateway

            yield return new PaymentProcessed(command.AggregateId, command.OrderId, command.Amount, command.PaymentMethod, isSuccessful, Version + 1);//, Version + 1);
        }
    }
}