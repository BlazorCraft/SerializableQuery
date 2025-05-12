using BlazorCraft.SerializableQuery;
using BlazorCraft.SerializableQuery.ExpressionConverter;
using BlazorCraft.SerializableQuery.Expressions;
using BlazorCraft.SerializableQuery.Sorting;
using Xunit;

namespace BlazorCraft.SerializableQuery.UnitTests;

public class BcDecimalExpressionConverterTests(ITestOutputHelper output) : BaseExpressionConverterTests(output)
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
                FilterAction = BcDecimalExpressionConverter.Equals,
                PropertyExpression = "Price",
                PropertyValue = "5000.615",
                PropertyType = typeof(decimal).FullName!
            }
        };

        var data = await RunFilterAsync(filter);
        Assert.Single(data);
        Assert.Equal(5000.615m, data[0].Price);
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
                FilterAction = BcDecimalExpressionConverter.NotEquals,
                PropertyExpression = "Price",
                PropertyValue = "5000.615",
                PropertyType = typeof(decimal).FullName!
            }
        };

        var data = await RunFilterAsync(filter);
        Assert.Equal(9, data.Count);
        Assert.DoesNotContain(data, d => d.Price == 5000.615m);
    }

    [Fact]
    public async Task GreaterThanOrEqual()
    {
        BcSerializableQuery filter = new BcSerializableQuery()
        {
            Page = 1,
            PageSize = Int32.MaxValue,
            SortOptions =
            [
                new()
                {
                    PropertyTree = "Price",
                    SortDirection = BcSerializableQuerySortDirection.Desc,
                }
            ],
            SerializableQuery = new BcSerializableQueryExpression()
            {
                ExpressionType = BcSerializableQueryExpressionType.AndExpression,
                Expressions =
                [
                    new()
                    {
                        ExpressionType = BcSerializableQueryExpressionType.FilterExpression,
                        FilterAction = BcDecimalExpressionConverter.GreaterThanOrEqual,
                        PropertyExpression = "Price",
                        PropertyValue = "5000",
                        PropertyType = typeof(decimal).FullName ?? throw new InvalidOperationException(),
                    }
                ]
            }
        };
        
        var data = await RunFilterAsync(filter);
        Assert.True(data.Count == 6);
    }

    [Fact]
    public async Task NotGreaterThanOrEqual()
    {
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType = BcSerializableQueryExpressionType.FilterExpression,
                FilterAction = BcDecimalExpressionConverter.NotGreaterThanOrEqual,
                PropertyExpression = "Price",
                PropertyValue = "5000",
                PropertyType = typeof(decimal).FullName!
            }
        };

        var data = await RunFilterAsync(filter);
        Assert.Equal(4, data.Count);
        Assert.All(data, d => Assert.True(d.Price < 5000m));
    }

    [Fact]
    public async Task LessThanOrEqual()
    {
        BcSerializableQuery filter = new BcSerializableQuery()
        {
            Page = 1,
            PageSize = Int32.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression()
            {
                ExpressionType = BcSerializableQueryExpressionType.AndExpression,
                Expressions =
                [
                    new()
                    {
                        ExpressionType = BcSerializableQueryExpressionType.FilterExpression,
                        FilterAction = "LessThanOrEqual",
                        PropertyExpression = "Price",
                        PropertyValue = "5000",
                        PropertyType = "System.Decimal"
                    }
                ]
            }
        };

        var data = await RunFilterAsync(filter);
        Assert.True(data.Count == 4);
    }

    [Fact]
    public async Task NotLessThanOrEqual()
    {
    
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType = BcSerializableQueryExpressionType.FilterExpression,
                FilterAction = BcDecimalExpressionConverter.NotLessThanOrEqual,
                PropertyExpression = "Price",
                PropertyValue = "5000",
                PropertyType = typeof(decimal).FullName!
            }
        };

        var data = await RunFilterAsync(filter);
        Assert.Equal(6, data.Count);
        Assert.All(data, d => Assert.True(d.Price > 5000m));
    }
}