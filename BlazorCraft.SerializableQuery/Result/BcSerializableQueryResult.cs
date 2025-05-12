namespace BlazorCraft.SerializableQuery.Result;

public class BcSerializableQueryResult<T> : IBcSerializableQueryResult<T>
{
    public int FilteredDataCount { get; init; }

    public int TotalDataCount { get; init; }

    public required List<T> Data { get; init; }
}