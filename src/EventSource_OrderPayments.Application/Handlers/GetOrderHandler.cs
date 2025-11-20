using Microsoft.EntityFrameworkCore;  // Add this
using EventSource_OrderPayments.Application.Queries;
using EventSource_OrderPayments.Infrastructure.ReadModels;
using MediatR;
using EventSource_OrderPayments.Infrastructure;

namespace EventSource_OrderPayments.Application.Handlers
{
    public class GetOrderHandler : IRequestHandler<GetOrder, OrderReadModel>
    {
        private readonly ApplicationDbContext _context;

        public GetOrderHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OrderReadModel> Handle(GetOrder request, CancellationToken cancellationToken)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);
        }
    }
}