namespace Base.Domain
{
    public class OperationLogEntity : Entity
    {
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public OperationLogEntity()
        {
        }
        public OperationLogEntity(Guid createdBy, DateTime createdOn)
        {
            CreatedBy = createdBy != Guid.Empty ? createdBy : Guid.Empty;
            CreatedOn = createdOn;
        }
        public OperationLogEntity(Guid createdBy, DateTime createdOn, Guid? updatedBy, DateTime? updatedOn)
        {
            CreatedBy = createdBy != Guid.Empty ? createdBy : Guid.Empty;
            CreatedOn = createdOn;
            if (updatedBy != Guid.Empty) UpdatedBy = updatedBy;
            UpdatedOn = updatedOn;
        }
    }
}
