namespace Base.Domain
{
    public class AuditDomain : DomainBase
    {
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        protected void SetIsDeleted(bool isDeleted)
        {
            IsDeleted = isDeleted;
        }
        protected void SetIsNew(bool isnew)
        {
            IsNew = isnew;
        }
        protected void SetId(long id)
        {
            Id = id;
        }
    }
}
