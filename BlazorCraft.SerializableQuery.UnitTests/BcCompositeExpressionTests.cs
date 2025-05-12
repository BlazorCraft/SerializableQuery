using BlazorCraft.SerializableQuery;
using BlazorCraft.SerializableQuery.ExpressionConverter;
using BlazorCraft.SerializableQuery.Expressions;
using Xunit;

namespace BlazorCraft.SerializableQuery.UnitTests;

public class BcCompositeExpressionTests(ITestOutputHelper output) :  BaseExpressionConverterTests(output)
{
    [Fact]
    public async Task Test_AndExpression()
    {
        // Status == Odd AND InStock == true
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType = BcSerializableQueryExpressionType.AndExpression,
                Expressions = new List<BcSerializableQueryExpression>
                {
                    // erste Teil-Expression: Enum-Filter auf "Odd"
                    new BcSerializableQueryExpression
                    {
                        ExpressionType = BcSerializableQueryExpressionType.FilterExpression,
                        PropertyExpression = "Status",
                        PropertyType = typeof(Enum).FullName!,
                        FilterAction = BcEnumExpressionConverter.Equals,
                        PropertyValue = "Odd"
                    },
                    // zweite Teil-Expression: Bool-Filter auf "True"
                    new BcSerializableQueryExpression
                    {
                        ExpressionType = BcSerializableQueryExpressionType.FilterExpression,
                        PropertyExpression = "InStock",
                        PropertyType = typeof(bool).FullName!,
                        FilterAction = BcBoolExpressionConverter.Equals,
                        PropertyValue = "True"
                    }
                }
            }
        };

        var data = await RunFilterAsync(filter);
        // Erwartet: nur die Artikel mit ungeradem Index (1,3,5,7,9) → 5 Einträge
        Assert.Equal(5, data.Count);
    }

    [Fact]
    public async Task Test_OrExpression()
    {
        // Status == Odd OR InStock == false
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType = BcSerializableQueryExpressionType.OrExpression,
                Expressions = new List<BcSerializableQueryExpression>
                {
                    // Teil-Expression: Enum-Filter auf "Odd"
                    new BcSerializableQueryExpression
                    {
                        ExpressionType = BcSerializableQueryExpressionType.FilterExpression,
                        PropertyExpression = "Status",
                        PropertyType = typeof(Enum).FullName!,
                        FilterAction = BcEnumExpressionConverter.Equals,
                        PropertyValue = "Odd"
                    },
                    // Teil-Expression: Bool-Filter auf "False"
                    new BcSerializableQueryExpression
                    {
                        ExpressionType = BcSerializableQueryExpressionType.FilterExpression,
                        PropertyExpression = "InStock",
                        PropertyType = typeof(bool).FullName!,
                        FilterAction = BcBoolExpressionConverter.Equals,
                        PropertyValue = "False"
                    }
                }
            }
        };

        var data = await RunFilterAsync(filter);
        // Erwartet: ungerade (1,3,5,7,9) plus instock==false (Index 0) → 6 Einträge
        Assert.Equal(6, data.Count);
    }
}