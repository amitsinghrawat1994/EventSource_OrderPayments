

using EventSource_OrderPayments.Domain.Events;
using MassTransit;

namespace EventSource_OrderPayments.Infrastructure.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreated>
    {
        public async Task Consume(ConsumeContext<OrderCreated> context)
        {
            var message = context.Message;

            // Business logic: e.g., send welcome email, log, or update external system
            Console.WriteLine($"Order created: {message.OrderId}");
        }
    }
}