using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Domain.LookUps
{
    public class LookUp : DomainBase
    {
        [Required]
        public long LookupTypeId { get; set; }

        [Required]
        [StringLength(256)]
        public required string Name { get; set; }

        [Required]
        [StringLength(128)]
        public required string Alias { get; set; }

        public int RelativeOrder { get; set; }


        [ForeignKey("LookupTypeId")]
        public virtual required LookupType LookupType { get; set; }
        
    }
}
