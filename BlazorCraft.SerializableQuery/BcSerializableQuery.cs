using BlazorCraft.SerializableQuery.Expressions;
using BlazorCraft.SerializableQuery.Sorting;

namespace BlazorCraft.SerializableQuery;

public class BcSerializableQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public BcSerializableQueryExpression? SerializableQuery { get; set; }
    public List<BcSerializableQuerySortOption> SortOptions { get; set; } = [];
}