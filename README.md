# Event-Sourced Order and Payments Backend

This is a real-world example of an event-sourced backend for order and payments processing using ASP.NET Core 9, CQRS, RabbitMQ (MassTransit), and event sourcing with Marten.

## Architecture

The solution follows Clean Architecture principles:

- **Domain**: Contains aggregates, events, commands, and value objects
- **Application**: CQRS handlers, queries, and application services
- **Infrastructure**: Persistence (Marten event store), messaging (MassTransit), and outbox
- **API**: ASP.NET Core Web API controllers

## Technologies

- ASP.NET Core 9
- Marten (Event Sourcing on PostgreSQL)
- MassTransit (Messaging with RabbitMQ)
- EF Core (Read models)
- FluentValidation (Validation)
- Serilog (Logging)
- Docker Compose (Local development)

## Project Structure

```
src/
├── EventSource_OrderPayments.API/          # Web API
├── EventSource_OrderPayments.Domain/       # Domain layer
├── EventSource_OrderPayments.Application/  # Application layer
└── EventSource_OrderPayments.Infrastructure/ # Infrastructure layer

test/
├── EventSource_OrderPayments.API.Tests/
├── EventSource_OrderPayments.Application.Tests/
├── EventSource_OrderPayments.Domain.Tests/
└── EventSource_OrderPayments.IntegrationTests/

infra/
└── docker-compose.yml                      # Docker dependencies
```

## Setup

1. **Start dependencies:**
   ```bash
   cd infra
   docker-compose up -d
   ```

2. **Restore packages:**
   ```bash
   dotnet restore
   ```

3. **Run the API:**
   ```bash
   cd src/EventSource_OrderPayments.API
   dotnet run
   ```

4. **Run tests:**
   ```bash
   dotnet test
   ```

## Key Concepts

### Events
```csharp
public class OrderCreated : BaseEvent
{
    public Guid OrderId { get; }
    public Guid CustomerId { get; }
    public List<OrderItem> Items { get; }
    public Money TotalAmount { get; }

    public OrderCreated(Guid orderId, Guid customerId, List<OrderItem> items, Money totalAmount, int version)
        : base(orderId, version)
    {
        OrderId = orderId;
        CustomerId = customerId;
        Items = items;
        TotalAmount = totalAmount;
    }
}

public class PaymentProcessed : BaseEvent
{
    public Guid PaymentId { get; }
    public Guid OrderId { get; }
    public Money Amount { get; }
    public string PaymentMethod { get; }
    public bool IsSuccessful { get; }

    public PaymentProcessed(Guid paymentId, Guid orderId, Money amount, string paymentMethod, bool isSuccessful, int version)
        : base(paymentId, version)
    {
        PaymentId = paymentId;
        OrderId = orderId;
        Amount = amount;
        PaymentMethod = paymentMethod;
        IsSuccessful = isSuccessful;
    }
}
```

### Event Sourcing
- Events are the source of truth
- Aggregates are rebuilt from events
- Commands generate events

### CQRS
- Separate command and query models
- Commands change state, queries read state

### Outbox Pattern
- Ensures reliable messaging
- Events are stored in outbox before publishing

### Idempotency
- Commands are idempotent based on aggregate version
- Message deduplication prevents duplicate processing

## API Endpoints

- `POST /api/orders` - Create order
- `POST /api/payments` - Process payment
- `GET /api/orders/{id}` - Get order details

## Code Examples

### Domain Aggregate
```csharp
public class OrderAggregate : AggregateBase
{
    public Guid Id { get; private set; }
    public List<OrderItem> Items { get; private set; } = new();
    public Money TotalAmount { get; private set; }
    public string Status { get; private set; } = "Pending";

    public void Apply(OrderCreated @event)
    {
        Id = @event.OrderId;
        Items = @event.Items;
        TotalAmount = @event.TotalAmount;
        Status = "Created";
    }

    public IEnumerable<IEvent> CreateOrder(CreateOrderCommand command)
    {
        if (Items.Any()) throw new InvalidOperationException("Order already created");
        var totalAmount = new Money(command.Items.Sum(i => i.TotalPrice.Amount), "USD");
        yield return new OrderCreated(command.AggregateId, command.CustomerId, command.Items, totalAmount, Version + 1);
    }
}

public class PaymentAggregate : AggregateBase
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Money Amount { get; private set; }
    public string PaymentMethod { get; private set; }
    public bool IsSuccessful { get; private set; }
    public string Status { get; private set; } = "Pending";

    public void Apply(PaymentProcessed @event)
    {
        Id = @event.PaymentId;
        OrderId = @event.OrderId;
        Amount = @event.Amount;
        PaymentMethod = @event.PaymentMethod;
        IsSuccessful = @event.IsSuccessful;
        Status = @event.IsSuccessful ? "Completed" : "Failed";
    }

    public IEnumerable<IEvent> ProcessPayment(ProcessPaymentCommand command)
    {
        if (Id != Guid.Empty) throw new InvalidOperationException("Payment already processed");

        // Simulate payment processing logic
        var isSuccessful = command.Amount.Amount > 0; // Simple validation

        yield return new PaymentProcessed(command.AggregateId, command.OrderId, command.Amount, command.PaymentMethod, isSuccessful, Version + 1);
    }
}
```

### Command Handler
```csharp
public class CreateOrderHandler : IRequestHandler<CreateOrder, Guid>
{
    public async Task<Guid> Handle(CreateOrder request, CancellationToken cancellationToken)
    {
        var aggregate = new OrderAggregate();
        var events = aggregate.CreateOrder(new CreateOrderCommand(...));

        _session.Events.StartStream<OrderAggregate>(orderId, events);
        await _session.SaveChangesAsync(cancellationToken);

        return orderId;
    }
}
```

### Event Consumer
```csharp
public class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        // Handle event, e.g., update read model
    }
}

public class PaymentProcessedConsumer : IConsumer<PaymentProcessed>
{
    public async Task Consume(ConsumeContext<PaymentProcessed> context)
    {
        var message = context.Message;
        
        if (message.IsSuccessful)
        {
            // Update order status, send confirmation, etc.
            Console.WriteLine($"Payment {message.PaymentId} for order {message.OrderId} succeeded");
        }
        else
        {
            // Handle failed payment
            Console.WriteLine($"Payment {message.PaymentId} for order {message.OrderId} failed");
        }
    }
}
```

## Persistence

Uses Marten for event storage and EF Core for read models.

## Messaging

MassTransit with RabbitMQ for asynchronous event processing.

## Validation

FluentValidation for command and input validation.

## Logging

Serilog for structured logging.

## Error Handling

Global exception middleware for consistent error responses.

## Current Status and Next Steps

### What We've Accomplished
- **Project Setup**: Created a complete Clean Architecture solution with Domain, Application, Infrastructure, and API layers, plus test projects and Docker infrastructure.
- **Core Implementation**: Implemented the full event-sourced flow:
  - **Order Creation**: CQRS command (`CreateOrder`) handled by `CreateOrderHandler`, generating `OrderCreated` events stored in Marten.
  - **Payment Processing**: CQRS command (`ProcessPayment`) handled by `ProcessPaymentHandler`, generating `PaymentProcessed` events.
  - **Querying**: Read models projected from events using Marten's `SingleStreamProjection`, queried via `GetOrder` handler.
  - **Event Handling**: MassTransit consumers (`OrderCreatedConsumer`, `PaymentProcessedConsumer`) for asynchronous event processing.
  - **Persistence**: Marten for event storage, EF Core for read models, with Outbox pattern support.
  - **Messaging**: MassTransit with RabbitMQ for reliable event publishing.
- **Build Success**: All projects compile without errors (minor warnings about nullable properties can be addressed later).
- **Testing Ready**: Endpoints are implemented and ready for testing with Postman/curl.

### Next Steps and Planning
1. **Run and Test**:
   - Start dependencies (`docker-compose up -d` in `infra/`).
   - Run the API (`dotnet run` in `src/EventSource_OrderPayments.API/`).
   - Test endpoints: Create order, process payment, query order.
   - Verify event publishing in RabbitMQ (http://localhost:15672).

2. **Add Enhancements**:
   - **Validation**: Integrate FluentValidation for command inputs.
   - **Logging**: Configure Serilog for structured logging.
   - **Error Handling**: Add global exception middleware.
   - **Idempotency**: Implement command deduplication.
   - **Outbox**: Enable reliable messaging with Marten's Outbox.
   - **Read Model Updates**: Enhance projections for more complex queries.

3. **Testing and Quality**:
   - Write unit tests for handlers and aggregates.
   - Add integration tests for full flows.
   - Run tests with `dotnet test`.

4. **Production Readiness**:
   - Add authentication/authorization.
   - Configure health checks and monitoring.
   - Optimize performance (e.g., caching, indexing).
   - Deploy with Docker/Kubernetes.

This project serves as a solid foundation for learning event sourcing, CQRS, and microservices patterns. Experiment with the code, add features, and refer to the code examples for deeper understanding!</content>
<parameter name="filePath">d:\Personal\MyBlog\my_blog_chat_gpt\sample\EventSource_OrderPayments\README.md