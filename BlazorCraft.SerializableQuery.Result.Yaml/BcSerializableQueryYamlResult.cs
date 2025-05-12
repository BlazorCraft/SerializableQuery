using BlazorCraft.SerializableQuery.Result;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BlazorCraft.SerializableQuery.Result.Yaml;

public class BcSerializableQueryYamlResult<T>(IBcSerializableQueryResult<T> result, INamingConvention? convention = null) : IBcSerializableQueryResult<T>
{
    private readonly ISerializer _serializer = new SerializerBuilder()
        .WithNamingConvention(convention ?? CamelCaseNamingConvention.Instance)
        .Build();

    private readonly IDeserializer _deserializer = new DeserializerBuilder()
        .WithNamingConvention(convention ?? CamelCaseNamingConvention.Instance)
        .Build();

    public string FilteredData => _serializer.Serialize(result.Data);
    public int FilteredDataCount => result.FilteredDataCount;
    public int TotalDataCount => result.TotalDataCount;
    public List<T> Data => _deserializer.Deserialize<List<T>>(FilteredData);

    public override string ToString()
    {
        return $"Result:{Environment.NewLine}{FilteredData}";
    }
    
    public static implicit operator BcSerializableQueryYamlResult<T>(BcSerializableQueryResult<T> result)
    {
        return new BcSerializableQueryYamlResult<T>(result);
    }
    
    public static implicit operator BcSerializableQueryResult<T>(BcSerializableQueryYamlResult<T> result)
    {
        return new BcSerializableQueryResult<T>()
        {
            Data = result.Data,
            FilteredDataCount = result.FilteredDataCount,
            TotalDataCount = result.TotalDataCount,
        };
    }

}