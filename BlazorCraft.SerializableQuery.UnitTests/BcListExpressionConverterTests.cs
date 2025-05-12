using BlazorCraft.SerializableQuery;
using BlazorCraft.SerializableQuery.ExpressionConverter;
using BlazorCraft.SerializableQuery.Expressions;
using BlazorCraft.SerializableQuery.UnitTests.Models;
using Xunit;

namespace BlazorCraft.SerializableQuery.UnitTests;

public class BcListExpressionConverterTests(ITestOutputHelper output) : BaseExpressionConverterTests(output)
{
    [Fact]
    public async Task Test_Any_IndexEquals5()
    {
        // Erwartet genau den einen Artikel, dessen ArticleCultures einen Eintrag mit Index == 5 enthält
        var filter = new BcSerializableQuery
        {
            Page     = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType     = BcSerializableQueryExpressionType.FilterListExpression,
                PropertyExpression = nameof(ArticleDto.ArticleCultures),
                PropertyType       = typeof(List<ArticleCultureDto>).FullName!,
                FilterAction       = BcListExpressionConverter.Any,
                Expressions =
                [
                    new()
                    {
                        ExpressionType = BcSerializableQueryExpressionType.FilterExpression,
                        PropertyExpression = nameof(ArticleCultureDto.Index),
                        PropertyType = typeof(int).FullName!,
                        FilterAction = BcIntegerExpressionConverter.Equals,
                        PropertyValue = "5"
                    }
                ]

            }
        };

        var data = await RunFilterAsync(filter);
        Assert.Single(data);
        Assert.Contains(data, dto => dto.ArticleCultures.Any(c => c.Index == 5));
    }

    [Fact]
    public async Task Test_All_IndexLessOrEqual3()
    {
        // Erwartet genau den einen Artikel, bei dem *alle* ArticleCultures.Index ≤ 3 sind (das ist Artikel 0)
        var filter = new BcSerializableQuery
        {
            Page     = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType     = BcSerializableQueryExpressionType.FilterListExpression,
                PropertyExpression = nameof(ArticleDto.ArticleCultures),
                PropertyType       = typeof(List<ArticleCultureDto>).FullName!,
                FilterAction       = BcListExpressionConverter.All,
                Expressions =
                [
                    new()
                    {
                        ExpressionType = BcSerializableQueryExpressionType.FilterExpression,
                        PropertyExpression = nameof(ArticleCultureDto.Index),
                        PropertyType = typeof(int).FullName!,
                        FilterAction = BcIntegerExpressionConverter.LessThanOrEqual,
                        PropertyValue = "3"
                    }
                ]
            }
        };

        var data = await RunFilterAsync(filter);
        Assert.Single(data);
        // Artikel 0 hat Indizes 0,1,2,3 → erfüllt die Bedingung
        Assert.All(data[0].ArticleCultures, c => Assert.True(c.Index <= 3));
    }
}