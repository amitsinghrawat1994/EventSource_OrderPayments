using EventSource_OrderPayments.Domain.ValueObjects;
using MediatR;

namespace EventSource_OrderPayments.Application.Commands
{
    public class CreateOrder : IRequest<Guid>
    {
        public Guid CustomerId { get; set; }
        public List<OrderItem> Items { get; } = new();
    }
}