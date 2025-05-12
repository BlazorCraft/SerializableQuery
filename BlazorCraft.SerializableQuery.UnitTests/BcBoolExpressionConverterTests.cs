using BlazorCraft.SerializableQuery;
using BlazorCraft.SerializableQuery.ExpressionConverter;
using BlazorCraft.SerializableQuery.Expressions;
using BlazorCraft.SerializableQuery.UnitTests.Models;
using Xunit;

namespace BlazorCraft.SerializableQuery.UnitTests;

public class BcBoolExpressionConverterTests(ITestOutputHelper output) : BaseExpressionConverterTests(output)
{
    [Fact]
    public async Task Test_Equals_True()
    {
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType     = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = nameof(ArticleDto.InStock),
                PropertyType       = typeof(bool).FullName!,
                FilterAction       = BcBoolExpressionConverter.Equals,
                PropertyValue      = "true"
            }
        };

        var data = await RunFilterAsync(filter);
        // InStock == true für i=1..9 → 9 Elemente
        Assert.Equal(9, data.Count);
        Assert.All(data, dto => Assert.True(dto.InStock));
    }

    [Fact]
    public async Task Test_Equals_False()
    {
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType     = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = nameof(ArticleDto.InStock),
                PropertyType       = typeof(bool).FullName!,
                FilterAction       = BcBoolExpressionConverter.Equals,
                PropertyValue      = "false"
            }
        };

        var data = await RunFilterAsync(filter);
        // InStock == false nur für i=0 → 1 Element
        Assert.Single(data);
        Assert.False(data[0].InStock);
    }

    [Fact]
    public async Task Test_NotEquals_True()
    {
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType     = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = nameof(ArticleDto.InStock),
                PropertyType       = typeof(bool).FullName!,
                FilterAction       = BcBoolExpressionConverter.NotEquals,
                PropertyValue      = "true"
            }
        };

        var data = await RunFilterAsync(filter);
        // InStock != true → nur InStock == false → 1 Element
        Assert.Single(data);
        Assert.False(data[0].InStock);
    }

    [Fact]
    public async Task Test_NotEquals_False()
    {
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType     = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = nameof(ArticleDto.InStock),
                PropertyType       = typeof(bool).FullName!,
                FilterAction       = BcBoolExpressionConverter.NotEquals,
                PropertyValue      = "false"
            }
        };

        var data = await RunFilterAsync(filter);
        // InStock != false → InStock == true für i=1..9 → 9 Elemente
        Assert.Equal(9, data.Count);
        Assert.All(data, dto => Assert.True(dto.InStock));
    }
}