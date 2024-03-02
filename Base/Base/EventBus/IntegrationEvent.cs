namespace Base.Infrastructure.EventBus
{
    public class IntegrationEvent 
    {
        public Guid Id { get; }=Guid.NewGuid();

        public DateTime OccurredOn { get; }=DateTime.UtcNow;
        public Guid UserId { get; set; }
    }
}