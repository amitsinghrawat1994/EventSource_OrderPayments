using System;
using EventSource_OrderPayments.Domain.Events;
using MassTransit;

namespace EventSource_OrderPayments.Infrastructure.Consumers;

public class PaymentProcessedConsumer : IConsumer<PaymentProcessed>
{
    public async Task Consume(ConsumeContext<PaymentProcessed> context)
    {
        var message = context.Message;

        if (message.IsSuccessful)
        {
            // Business logic for successful payment
            Console.WriteLine($"Payment processed successfully: {message.PaymentId} for Order: {message.OrderId}, Amount: {message.Amount}");
        }
        else
        {
            // Business logic for failed payment
            Console.WriteLine($"Payment processing failed: {message.PaymentId} for Order: {message.OrderId}, Amount: {message.Amount}");
        }
    }
}
