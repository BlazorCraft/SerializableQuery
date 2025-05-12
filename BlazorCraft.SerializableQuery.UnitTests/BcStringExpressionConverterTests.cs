using BlazorCraft.SerializableQuery;
using BlazorCraft.SerializableQuery.ExpressionConverter;
using BlazorCraft.SerializableQuery.Expressions;
using BlazorCraft.SerializableQuery.UnitTests.Models;
using Xunit;

namespace BlazorCraft.SerializableQuery.UnitTests;

public class BcStringExpressionConverterTests(ITestOutputHelper output) : BaseExpressionConverterTests(output)
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
                ExpressionType    = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = nameof(ArticleDto.ArticleNumber),
                PropertyType       = typeof(string).FullName!,
                FilterAction       = BcStringExpressionConverter.Equals,
                PropertyValue      = "A0003"
            }
        };

        var data = await RunFilterAsync(filter);
        Assert.Equal(1, data.Count);
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
                ExpressionType    = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = nameof(ArticleDto.ArticleNumber),
                PropertyType       = typeof(string).FullName!,
                FilterAction       = BcStringExpressionConverter.NotEquals,
                PropertyValue      = "A0003"
            }
        };

        var data = await RunFilterAsync(filter);
        Assert.Equal(9, data.Count);
    }

    [Fact]
    public async Task Test_Contains()
    {
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType    = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = nameof(ArticleDto.ArticleNumber),
                PropertyType       = typeof(string).FullName!,
                FilterAction       = BcStringExpressionConverter.Contains,
                PropertyValue      = "0003"
            }
        };

        var data = await RunFilterAsync(filter);
        Assert.Equal(1, data.Count);
    }

    [Fact]
    public async Task Test_NotContains()
    {
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType    = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = nameof(ArticleDto.ArticleNumber),
                PropertyType       = typeof(string).FullName!,
                FilterAction       = BcStringExpressionConverter.NotContains,
                PropertyValue      = "0003"
            }
        };

        var data = await RunFilterAsync(filter);
        Assert.Equal(9, data.Count);
    }

    [Fact]
    public async Task Test_StartsWith()
    {
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType    = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = nameof(ArticleDto.ArticleNumber),
                PropertyType       = typeof(string).FullName!,
                FilterAction       = BcStringExpressionConverter.StartsWith,
                PropertyValue      = "A0003"
            }
        };

        var data = await RunFilterAsync(filter);
        Assert.Equal(1, data.Count);
    }

    [Fact]
    public async Task Test_NotStartsWith()
    {
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType    = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = nameof(ArticleDto.ArticleNumber),
                PropertyType       = typeof(string).FullName!,
                FilterAction       = BcStringExpressionConverter.NotStartsWith,
                PropertyValue      = "A0003"
            }
        };

        var data = await RunFilterAsync(filter);
        Assert.Equal(9, data.Count);
    }

    [Fact]
    public async Task Test_EndsWith()
    {
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType    = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = nameof(ArticleDto.ArticleNumber),
                PropertyType       = typeof(string).FullName!,
                FilterAction       = BcStringExpressionConverter.EndsWith,
                PropertyValue      = "3"
            }
        };

        var data = await RunFilterAsync(filter);
        Assert.Equal(1, data.Count);
    }

    [Fact]
    public async Task Test_NotEndsWith()
    {
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType    = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = nameof(ArticleDto.ArticleNumber),
                PropertyType       = typeof(string).FullName!,
                FilterAction       = BcStringExpressionConverter.NotEndsWith,
                PropertyValue      = "3"
            }
        };

        var data = await RunFilterAsync(filter);
        Assert.Equal(9, data.Count);
    }
}
