namespace BlazorCraft.SerializableQuery.Result;

public interface IBcSerializableQueryResult<T>
{
    /// <summary>
    /// The data after filtering and paging.
    /// </summary>
    public List<T> Data { get; }
    
    /// <summary>
    /// The total count of data.
    /// </summary>
    public int TotalDataCount { get; }
    
    /// <summary>
    /// The Data after filtering but before paging.
    /// </summary>
    public int FilteredDataCount { get; }
}