using BuildingBlocks.Filters;
using System.ComponentModel.DataAnnotations;

namespace Shared.Model
{
    public class PagingRequestModel
    {
        public int Page { get; set; }
        [Range(1, 100,ErrorMessage ="PerPage value must be between 1 to 100")]
        public int PerPage { get; set; } = 10;
        public string? SortBy { get; set; } = "";
        public int? SortDirection { get; set; }
        public List<FilterCriteria>? Filters { get; set; }
        public bool IsApi { get; set; } = false;

    }
}
