
using EventSource_OrderPayments.Infrastructure.ReadModels;
using MediatR;

namespace EventSource_OrderPayments.Application.Queries
{
    public class GetOrder : IRequest<OrderReadModel>
    {
        public Guid OrderId { get; set; }
    }
}