namespace Base.Filters;

public class FilterCriteria
{
    public FilterCriteria()
    {
    }

    public FilterCriteria(string field, FieldTypeEnum FieldType, LogicOperationEnum logicOperation, ComparisonOperationEnum comparisionOperation,   List<object> values)
    {
        Field = field;
        LogicOperation = logicOperation;
        ComparisonOperation = comparisionOperation;
        Values = values;
    }

    public string Field { get; set; }
    public List<object> Values { get; set; }
    public LogicOperationEnum LogicOperation { get; set; }
    public ComparisonOperationEnum ComparisonOperation { get; set; }
       
}
