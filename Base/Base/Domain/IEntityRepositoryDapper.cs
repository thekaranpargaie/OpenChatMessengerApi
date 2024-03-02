namespace Base.Domain;

public class FieldQuery
{
    public required string SchemaName;
    public required string Table;
    public required string Field;
    public required string searchText;
    public required string ClientId;
}


public interface IEntityRepositoryDapper 
{
    public Task<IEnumerable<string>> GetDistinctListByFieldName(FieldQuery fieldQuery);

}