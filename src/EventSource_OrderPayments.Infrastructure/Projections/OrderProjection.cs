using Marten.Events.Aggregation;
using EventSource_OrderPayments.Domain.Events;
using EventSource_OrderPayments.Infrastructure.ReadModels;

namespace EventSource_OrderPayments.Infrastructure.Projections
{
    public class OrderProjection : SingleStreamProjection<OrderReadModel, Guid>
    {
        public void Apply(OrderCreated @event, OrderReadModel view)
        {
            view.Id = @event.OrderId;
            view.CustomerId = @event.CustomerId;
            view.Items = @event.Items.Select(i => new OrderItemReadModel
            {
                Id = Guid.NewGuid(),
                OrderId = @event.OrderId,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.TotalPrice.Amount,
                Currency = i.TotalPrice.Currency
            }).ToList();
            view.TotalAmount = @event.TotalAmount.Amount;
            view.Currency = @event.TotalAmount.Currency;
            view.Status = "Created";
        }
    }
}