namespace EventSource_OrderPayments.Domain.ValueObjects
{
    public class OrderItem
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public Money TotalPrice { get; set; }
    }
}