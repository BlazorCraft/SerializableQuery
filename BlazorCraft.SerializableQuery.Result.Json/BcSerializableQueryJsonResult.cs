using System.Text.Json;

namespace BlazorCraft.SerializableQuery.Result.Json;

public class BcSerializableQueryJsonResult<T>(IBcSerializableQueryResult<T> result) : IBcSerializableQueryResult<T>
{
    public string FilteredData => JsonSerializer.Serialize(result.Data);
    public int FilteredDataCount => result.FilteredDataCount;
    public int TotalDataCount => result.TotalDataCount;
    public List<T> Data => JsonSerializer.Deserialize<List<T>>(FilteredData) ?? [];
    
    public override string ToString()
    {
        return $"Result:{Environment.NewLine}{FilteredData}";
    }
    
    public static implicit operator BcSerializableQueryResult<T>(BcSerializableQueryJsonResult<T> result)
    {
        return new BcSerializableQueryResult<T>()
        {
            Data = result.Data,
            FilteredDataCount = result.FilteredDataCount,
            TotalDataCount = result.TotalDataCount,
        };
    }
    
    public static implicit operator BcSerializableQueryJsonResult<T>(BcSerializableQueryResult<T> result)
    {
        return new BcSerializableQueryJsonResult<T>(result);
    }
}