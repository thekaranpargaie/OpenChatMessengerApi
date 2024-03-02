using System.Linq.Expressions;

namespace Base.Filters;

public interface IQueryFilterService
{
    IQueryable<T> FilterDataSet<T>(IQueryable<T> query, List<FilterCriteria> filterCriteriaList);
	Dictionary<string, int> GetWorkstreamStatusCounts<T>(IEnumerable<T> query, Func<T, string> getStatusFunc);


}