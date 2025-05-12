using BlazorCraft.SerializableQuery;
using BlazorCraft.SerializableQuery.ExpressionConverter;
using BlazorCraft.SerializableQuery.Expressions;
using BlazorCraft.SerializableQuery.UnitTests.Models;
using Xunit;

namespace BlazorCraft.SerializableQuery.UnitTests;

public class BcIntegerExpressionConverterTests(ITestOutputHelper output) : BaseExpressionConverterTests(output)
{
    [Fact]
    public async Task Test_Equals()
    {
        const int target = 300;
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType     = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = nameof(ArticleDto.StockAmount),
                PropertyType       = typeof(int).FullName!,
                FilterAction       = BcIntegerExpressionConverter.Equals,
                PropertyValue      = target.ToString()
            }
        };

        var data = await RunFilterAsync(filter);
        Assert.Single(data);
        Assert.Equal(target, data[0].StockAmount);
    }

    [Fact]
    public async Task Test_NotEquals()
    {
        const int target = 300;
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType     = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = nameof(ArticleDto.StockAmount),
                PropertyType       = typeof(int).FullName!,
                FilterAction       = BcIntegerExpressionConverter.NotEquals,
                PropertyValue      = target.ToString()
            }
        };

        var data = await RunFilterAsync(filter);
        Assert.Equal(9, data.Count);
        Assert.DoesNotContain(data, dto => dto.StockAmount == target);
    }

    [Fact]
    public async Task Test_GreaterThanOrEqual()
    {
        const int target = 300;
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType     = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = nameof(ArticleDto.StockAmount),
                PropertyType       = typeof(int).FullName!,
                FilterAction       = BcIntegerExpressionConverter.GreaterThanOrEqual,
                PropertyValue      = target.ToString()
            }
        };

        var data = await RunFilterAsync(filter);
        // 300, 400, 500, 600, 700, 800, 900 => 7 items
        Assert.Equal(7, data.Count);
        Assert.All(data, dto => Assert.True(dto.StockAmount >= target));
    }

    [Fact]
    public async Task Test_LessThanOrEqual()
    {
        const int target = 300;
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType     = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = nameof(ArticleDto.StockAmount),
                PropertyType       = typeof(int).FullName!,
                FilterAction       = BcIntegerExpressionConverter.LessThanOrEqual,
                PropertyValue      = target.ToString()
            }
        };

        var data = await RunFilterAsync(filter);
        // 0, 100, 200, 300 => 4 items
        Assert.Equal(4, data.Count);
        Assert.All(data, dto => Assert.True(dto.StockAmount <= target));
    }

    [Fact]
    public async Task Test_NotGreaterThanOrEqual()
    {
        const int target = 300;
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType     = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = nameof(ArticleDto.StockAmount),
                PropertyType       = typeof(int).FullName!,
                FilterAction       = BcIntegerExpressionConverter.NotGreaterThanOrEqual,
                PropertyValue      = target.ToString()
            }
        };

        var data = await RunFilterAsync(filter);
        // strictly less than 300 => 0,100,200 => 3 items
        Assert.Equal(3, data.Count);
        Assert.All(data, dto => Assert.True(dto.StockAmount < target));
    }

    [Fact]
    public async Task Test_NotLessThanOrEqual()
    {
        const int target = 300;
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType     = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = nameof(ArticleDto.StockAmount),
                PropertyType       = typeof(int).FullName!,
                FilterAction       = BcIntegerExpressionConverter.NotLessThanOrEqual,
                PropertyValue      = target.ToString()
            }
        };

        var data = await RunFilterAsync(filter);
        // strictly greater than 300 => 400,500,600,700,800,900 => 6 items
        Assert.Equal(6, data.Count);
        Assert.All(data, dto => Assert.True(dto.StockAmount > target));
    }
}