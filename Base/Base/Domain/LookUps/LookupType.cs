using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace Base.Domain.LookUps
{
    public class LookupType : DomainBase
    {
        [Required]
        [StringLength(128)]
        public required string Name { get; set; }

        [Required]
        [StringLength(64)]
        public required string Alias { get; set; }
    }
}
