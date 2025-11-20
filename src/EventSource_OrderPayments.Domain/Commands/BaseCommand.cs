namespace EventSource_OrderPayments.Domain.Commands
{
    public abstract class BaseCommand : ICommand
    {
        public Guid Id { get; }
        public Guid AggregateId { get; }

        protected BaseCommand(Guid aggregateId)
        {
            Id = Guid.NewGuid();
            AggregateId = aggregateId;
        }
    }
}