namespace EventSource_OrderPayments.Domain.Commands
{
    public interface ICommand
    {
        Guid Id { get; }
        Guid AggregateId { get; }
    }
}