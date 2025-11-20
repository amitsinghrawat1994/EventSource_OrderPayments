using EventSource_OrderPayments.Application.Commands;
using EventSource_OrderPayments.Domain.Aggregates;
using EventSource_OrderPayments.Domain.Commands;
using EventSource_OrderPayments.Domain.Events;
using Marten;
using MediatR;

namespace EventSource_OrderPayments.Application.Handlers
{
    public class CreateOrderHandler : IRequestHandler<CreateOrder, Guid>
    {
        private readonly IDocumentSession _session;

        public CreateOrderHandler(IDocumentSession session)
        {
            _session = session;
        }

        public async Task<Guid> Handle(CreateOrder request, CancellationToken cancellationToken)
        {
            var orderId = Guid.NewGuid();
            var command = new CreateOrderCommand(orderId, request.CustomerId, request.Items);

            var aggregate = new OrderAggregate();
            var events = aggregate.CreateOrder(command);

            _session.Events.StartStream<OrderAggregate>(orderId, events);
            await _session.SaveChangesAsync(cancellationToken);

            return orderId;
        }
    }
}