using Serilog;
using Serilog.Sinks.Seq;
using Microsoft.EntityFrameworkCore;
using Marten;
using Marten.AspNetCore;
using JasperFx.Events.Projections;  // Add this
using EventSource_OrderPayments.Infrastructure;
using EventSource_OrderPayments.Infrastructure.Projections;
using MassTransit;
using EventSource_OrderPayments.Infrastructure.Consumers;
using MediatR;
using EventSource_OrderPayments.Application.Handlers;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateOrderHandler).Assembly));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedConsumer>();
    x.AddConsumer<PaymentProcessedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMQConfig = builder.Configuration.GetSection("RabbitMQ");
        cfg.Host(rabbitMQConfig["Host"], "/", h =>
        {
            h.Username(rabbitMQConfig["Username"]);
            h.Password(rabbitMQConfig["Password"]);
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

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
