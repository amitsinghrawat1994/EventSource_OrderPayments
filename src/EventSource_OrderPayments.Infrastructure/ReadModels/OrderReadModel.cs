using System.ComponentModel.DataAnnotations;

namespace EventSource_OrderPayments.Infrastructure.ReadModels
{
    public class OrderReadModel
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public List<OrderItemReadModel> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
    }

    public class OrderItemReadModel
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Currency { get; set; }
    }
}