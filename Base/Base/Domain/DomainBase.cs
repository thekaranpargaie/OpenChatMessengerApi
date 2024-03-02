using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Domain
{
    public abstract class DomainBase
    {
        public virtual long Id { get; set; }

        [NotMapped]
        public bool IsNew { get; set; }

        public bool IsDeleted { get; set; }

        public virtual object[] GetId()
        {
            return new Object[] { Id };
        }
    }
}
