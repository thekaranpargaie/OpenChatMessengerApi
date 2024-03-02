using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Domain
{
    public abstract class Entity
    {
        private List<IDomainEvent> _domainEvents;

        /// <summary>
        /// Domain events occurred.
        /// </summary>
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

        /// <summary>
        /// Add domain event.
        /// </summary>
        /// <param name="domainEvent"></param>
        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents = _domainEvents ?? new List<IDomainEvent>();
            _domainEvents.Add(domainEvent);
        }

        /// <summary>
        /// Clear domain events.
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        protected static void CheckRule(IBusinessRule rule)
        {
            if (rule.IsBroken())
            {
                throw new BusinessRuleValidationException(rule);
            }
        }
        protected void SetId(Guid id)
        {
            Id = id;
        }
        protected void SetIsNew(bool isnew)
        {
            IsNew = isnew;
        }
        protected void SetIsDeleted(bool isDeleted)
        {
            IsDeleted = isDeleted;
        }

        public Guid Id { get; private set; }
        public bool IsDeleted { get; private set; }
        [NotMapped]
        public bool IsNew { get; private set; }
    }
}
