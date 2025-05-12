using BlazorCraft.SerializableQuery;
using BlazorCraft.SerializableQuery.ExpressionConverter;
using BlazorCraft.SerializableQuery.Expressions;
using Xunit;

namespace BlazorCraft.SerializableQuery.UnitTests;

public class BcEnumExpressionConverterTests(ITestOutputHelper output) : BaseExpressionConverterTests(output)
{
    [Fact]
    public async Task Test_Equals()
    {
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = "Status",
                PropertyType = typeof(Enum).FullName!,
                FilterAction = BcEnumExpressionConverter.Equals,
                PropertyValue = "Odd"
            }
        };

        var data = await RunFilterAsync(filter);
        Assert.Equal(5, data.Count);
    }
    
    [Fact]
    public async Task Test_NotEquals()
    {
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = "Status",
                PropertyType = typeof(Enum).FullName!,
                FilterAction = BcEnumExpressionConverter.NotEquals,
                PropertyValue = "Odd"
            }
        };

        var data = await RunFilterAsync(filter);
        Assert.Equal(5, data.Count);
    }
}