using EventSource_OrderPayments.Domain.ValueObjects;
using MediatR;

namespace EventSource_OrderPayments.Application.Commands
{
    public class ProcessPayment : IRequest<Guid>
    {
        public Guid OrderId { get; set; }
        public Money Amount { get; set; }
        public string PaymentMethod { get; set; }
    }
}