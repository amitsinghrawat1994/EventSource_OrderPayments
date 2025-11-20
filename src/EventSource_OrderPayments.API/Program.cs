using Microsoft.EntityFrameworkCore;
using Marten;
using Marten.AspNetCore;
using JasperFx.Events.Projections;  // Add this
using EventSource_OrderPayments.Infrastructure;
using EventSource_OrderPayments.Infrastructure.Projections;
using MassTransit;
using EventSource_OrderPayments.Infrastructure.Consumers;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedConsumer>();
    x.AddConsumer<PaymentProcessedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddMarten(provider =>
{
    var options = new StoreOptions();
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Host=localhost;Database=eventstore;Username=postgres;Password=password";
    options.Connection(connectionString);
    options.Projections.Add<OrderProjection>(ProjectionLifecycle.Inline);
    return options;
}).UseLightweightSessions();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
