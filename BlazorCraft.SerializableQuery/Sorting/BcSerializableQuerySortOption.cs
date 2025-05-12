namespace BlazorCraft.SerializableQuery.Sorting;

public class BcSerializableQuerySortOption
{
    public Guid TrackingId = Guid.NewGuid();
    public required string PropertyTree { get; set; }
    public BcSerializableQuerySortDirection SortDirection { get; set; }
}