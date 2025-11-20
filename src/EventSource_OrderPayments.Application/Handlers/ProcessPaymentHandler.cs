using EventSource_OrderPayments.Application.Commands;
using EventSource_OrderPayments.Domain.Aggregates;
using EventSource_OrderPayments.Domain.Commands;
using Marten;
using MediatR;

namespace EventSource_OrderPayments.Application.Handlers
{
    public class ProcessPaymentHandler : IRequestHandler<ProcessPayment, Guid>
    {
        private readonly IDocumentSession _session;

        public ProcessPaymentHandler(IDocumentSession session)
        {
            _session = session;
        }

        public async Task<Guid> Handle(ProcessPayment request, CancellationToken cancellationToken)
        {
            var paymentId = Guid.NewGuid();
            var command = new ProcessPaymentCommand(paymentId, request.OrderId, request.Amount, request.PaymentMethod);

            var aggregate = new PaymentAggregate();
            var events = aggregate.ProcessPayment(command);

            _session.Events.StartStream<PaymentAggregate>(paymentId, events);
            await _session.SaveChangesAsync(cancellationToken);

            return paymentId; // Assuming success for simplicity
        }
    }
}