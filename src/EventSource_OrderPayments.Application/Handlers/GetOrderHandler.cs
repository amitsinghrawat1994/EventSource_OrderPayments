using Marten;  // Add this
using EventSource_OrderPayments.Application.Queries;
using EventSource_OrderPayments.Infrastructure.ReadModels;
using MediatR;

namespace EventSource_OrderPayments.Application.Handlers
{
    public class GetOrderHandler : IRequestHandler<GetOrder, OrderReadModel>
    {
        private readonly IDocumentSession _session;

        public GetOrderHandler(IDocumentSession session)
        {
            _session = session;
        }

        public async Task<OrderReadModel> Handle(GetOrder request, CancellationToken cancellationToken)
        {
            return await _session.LoadAsync<OrderReadModel>(request.OrderId, cancellationToken);
        }
    }
}