using BlazorCraft.SerializableQuery;
using BlazorCraft.SerializableQuery.ExpressionConverter;
using BlazorCraft.SerializableQuery.Expressions;
using BlazorCraft.SerializableQuery.UnitTests.Models;
using Xunit;

namespace BlazorCraft.SerializableQuery.UnitTests;

public class DateTimeExpressionConverterTests(ITestOutputHelper output) : BaseExpressionConverterTests(output)
{
    [Fact]
    public async Task Test_Equals()
    {
        var entities = ArticleRepository.Read().ToList();
        // Wir wählen den 4. Eintrag (i=3), das ist Created = Heute.Date.AddDays(-3)
        var target = DateTime.Now.Date.AddDays(-3);
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType     = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = nameof(ArticleDto.Created),
                PropertyType       = typeof(DateTime).FullName!,
                FilterAction       = BcDateTimeExpressionConverter.Equals,
                PropertyValue      = target.ToString("yyyy-MM-dd")
            }
        };

        var data = await RunFilterAsync(filter);
        Assert.Single(data);
        Assert.Equal(target, data[0].Created.Date);
    }

    [Fact]
    public async Task Test_NotEquals()
    {
        var target = DateTime.Now.Date.AddDays(-3);
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType     = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = nameof(ArticleDto.Created),
                PropertyType       = typeof(DateTime).FullName!,
                FilterAction       = BcDateTimeExpressionConverter.NotEquals,
                PropertyValue      = target.ToString("yyyy-MM-dd")
            }
        };

        var data = await RunFilterAsync(filter);
        Assert.Equal(9, data.Count);
        Assert.DoesNotContain(data, dto => dto.Created.Date == target);
    }

    [Fact]
    public async Task Test_Greater()
    {
        // Alle Datensätze mit Created > target (also i = 0,1,2) → 3 Elemente
        var target = DateTime.Now.Date.AddDays(-3);
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType     = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = nameof(ArticleDto.Created),
                PropertyType       = typeof(DateTime).FullName!,
                FilterAction       = BcDateTimeExpressionConverter.Greater,
                PropertyValue      = target.ToString("yyyy-MM-dd")
            }
        };

        var data = await RunFilterAsync(filter);
        Assert.Equal(3, data.Count);
        Assert.All(data, dto => Assert.True(dto.Created.Date > target));
    }

    [Fact]
    public async Task Test_Lower()
    {
        // Alle Datensätze mit Created < target (also i = 4..9) → 6 Elemente
        var target = DateTime.Now.Date.AddDays(-3);
        var filter = new BcSerializableQuery
        {
            Page = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType     = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = nameof(ArticleDto.Created),
                PropertyType       = typeof(DateTime).FullName!,
                FilterAction       = BcDateTimeExpressionConverter.Lower,
                PropertyValue      = target.ToString("yyyy-MM-dd")
            }
        };

        var data = await RunFilterAsync(filter);
        Assert.Equal(6, data.Count);
        Assert.All(data, dto => Assert.True(dto.Created.Date < target));
    }
}