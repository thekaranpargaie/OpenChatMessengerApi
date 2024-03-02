using System.Linq.Expressions;
using System.Text.Json;


namespace Base.Filters
{
    public class FilterRequestModel
    {
        public List<FilterCriteria>? FilterCriteria;
    }


    public class QueryFilterService : IQueryFilterService
    {
        public const string BooleanSystemType = "System.Boolean";
        public QueryFilterService()
        {
        }

        public IQueryable<TEntity> FilterDataSet<TEntity>(IQueryable<TEntity> query, List<FilterCriteria> filterCriteriaList) 
        {
            if (filterCriteriaList.Count == 0)
            {
                return query;

            }

            filterCriteriaList = PreprocessFilterCriteria(filterCriteriaList);
 
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            Expression? finalExpression = null;

            foreach (var filterCriteria in filterCriteriaList)
            {
                var expression = BuildExpression(query, parameter, filterCriteria);
                if (finalExpression == null)
                {
                    finalExpression = expression;
                }
                else
                {
                    if (filterCriteria.LogicOperation == LogicOperationEnum.Or)
                    {
                        finalExpression = Expression.Or(finalExpression, expression);
                    }
                    else
                    {
                        finalExpression = Expression.And(finalExpression, expression);
                    }

                }
            }
            if (finalExpression != null)
            {
                var lambda = Expression.Lambda<Func<TEntity, bool>>(finalExpression, parameter);
                query = query.Where(lambda);
            }
            return query;
        }

        private List<FilterCriteria> PreprocessFilterCriteria(List<FilterCriteria> filterCriteriaList)
        {
            //modify the filter collection for between operation
            if (filterCriteriaList.Count == 0)
            {
                return filterCriteriaList;
            }
            //check to see if the first one is a between operation
            if (filterCriteriaList.First().ComparisonOperation == ComparisonOperationEnum.Between)
            {
                filterCriteriaList.First().LogicOperation = LogicOperationEnum.And; //force it to be an and operation 
            }
            var preProcess = filterCriteriaList.Where(x => x.ComparisonOperation == ComparisonOperationEnum.Between).ToList();
            foreach (FilterCriteria criteria in preProcess)
            {
                var values = criteria.Values;
                var firstValue = values[0];
                var secondValue = values[1];
                var newCriteria = new FilterCriteria()
                {
                    Field = criteria.Field,
                    ComparisonOperation = ComparisonOperationEnum.GreaterThan,
                    LogicOperation = criteria.LogicOperation,
                    Values = new List<Object>() { firstValue }
                };
                filterCriteriaList.Add(newCriteria);
                newCriteria = new FilterCriteria()
                {
                    Field = criteria.Field,
                    ComparisonOperation = ComparisonOperationEnum.LessThan,
                    LogicOperation = LogicOperationEnum.And,
                    Values = new List<Object>() { secondValue }
                };
                filterCriteriaList.Add(newCriteria);
               
            }
            filterCriteriaList.RemoveAll(x => x.ComparisonOperation == ComparisonOperationEnum.Between);

            return filterCriteriaList;


        }

        private Expression BuildExpression<TEntity>(IQueryable<TEntity> query,ParameterExpression parameter, FilterCriteria filterCriteria  )
        {
            var property = typeof(TEntity).GetProperty(filterCriteria.Field);
            var propertyAccess = Expression.Property(parameter, property);
            var filterExpression = GetFilterExpression(propertyAccess, filterCriteria );
            return filterExpression;
        }

        private IQueryable<TEntity> ApplyFilterCriteria<TEntity>(IQueryable<TEntity> query, FilterCriteria filterCriteria)
        {
            var property = typeof(TEntity).GetProperty(filterCriteria.Field);

            if (property != null)
            {
                var parameter = Expression.Parameter(typeof(TEntity), "x");
                var propertyAccess = Expression.Property(parameter, property);

                var filterExpression = GetFilterExpression(propertyAccess, filterCriteria);
                if (filterExpression != null)
                {
                    var lambda = Expression.Lambda<Func<TEntity, bool>>(filterExpression, parameter);
                    //temporarily placed here while I finalized the multiple field OR logic 
                    query = query.Where(lambda);
                }
            }
            return query;
        }

        private Expression GetFilterExpression(MemberExpression propertyAccess, FilterCriteria filterCriteria )
        {
            //generic lambda expression 
            Expression? filterExpression = null;

            foreach (var value in filterCriteria.Values)
            {
                Expression valueExpression = GetValueExpressionFromJSON(propertyAccess, (JsonElement)value, filterCriteria);
                if (filterExpression == null)
                {
                    filterExpression = AppendExpression(propertyAccess, filterCriteria, valueExpression);
                }
                else
                {
                    var appendExpression = AppendExpression(propertyAccess, filterCriteria, valueExpression);
                    if (filterCriteria.ComparisonOperation != ComparisonOperationEnum.NotEqual){
                        filterExpression = Expression.OrElse(filterExpression, appendExpression);
                    }
                    else
                    {
                        filterExpression = Expression.AndAlso(filterExpression, appendExpression);
                    }
                }                
            }
            return filterExpression;
        }

        private static BinaryExpression AppendExpression(MemberExpression propertyAccess, FilterCriteria filterCriteria,
            Expression valueExpression)
        {
            var appendExpression = filterCriteria.ComparisonOperation switch
            {
                ComparisonOperationEnum.Equal => Expression.Equal(propertyAccess, valueExpression),
                ComparisonOperationEnum.GreaterThan => Expression.GreaterThan(propertyAccess, valueExpression),
                ComparisonOperationEnum.LessThan => Expression.LessThan(propertyAccess, valueExpression),
                _ => Expression.NotEqual(propertyAccess, valueExpression)
            };
            return appendExpression;
        }

        private Expression GetValueExpressionFromJSON(MemberExpression propertyAccess, JsonElement value, FilterCriteria filterCriteria )
        {
            
            switch (value.ValueKind)
            {
                case JsonValueKind.String:
                    DateTime dt;
                    //check for date time
                    if (DateTime.TryParse(value.GetString(), out dt))
                    {
                        var adjustedDate = new DateTime();
                        if (filterCriteria.ComparisonOperation == ComparisonOperationEnum.GreaterThan)
                        {
                            adjustedDate = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, DateTimeKind.Utc);
                        }
                        else
                        {
                            adjustedDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, DateTimeKind.Utc);
                        }

                        return Expression.Constant(DateTime.SpecifyKind(adjustedDate, DateTimeKind.Utc)  , propertyAccess.Type);
                    }
                    else if(Guid.TryParse(value.GetString(), out var guidValue))
                    {
                        return Expression.Constant(guidValue, typeof(Guid));
                    }
                    else if (propertyAccess.Type.FullName.Contains(BooleanSystemType))
                    {
                        Nullable<bool>  returnvalue;
                        returnvalue = value.ToString().ToLower() == "true" ? true : false;
                        return Expression.Constant(returnvalue, typeof(bool?));
                    }
                    return Expression.Constant(value.GetString());
                case JsonValueKind.Number:
                    //May have to add a switch statement here 
                    return Expression.Constant(value.GetInt32());
                case JsonValueKind.True:
                    return Expression.Constant(true);
                case JsonValueKind.False:
                    return Expression.Constant(false);
                case JsonValueKind.Null:
                    return Expression.Constant(null);
                case JsonValueKind.Object:
                    return Expression.Constant(value.GetDateTime());
                default:
                    throw new NotSupportedException("Unsupported value type");
            }
        }
		
		public Dictionary<string, int> GetWorkstreamStatusCounts<T>(IEnumerable<T> query, Func<T, string> getStatusFunc)
		{
			return query
				.GroupBy(x => getStatusFunc(x))
				.ToDictionary(g => g.Key, g => g.Count());
		}
	}
}
