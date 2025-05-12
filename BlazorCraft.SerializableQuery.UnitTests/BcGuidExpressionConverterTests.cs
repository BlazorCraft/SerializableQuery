using BlazorCraft.SerializableQuery;
using BlazorCraft.SerializableQuery.ExpressionConverter;
using BlazorCraft.SerializableQuery.Expressions;
using BlazorCraft.SerializableQuery.UnitTests.Models;
using Xunit;

namespace BlazorCraft.SerializableQuery.UnitTests;

public class BcGuidExpressionConverterTests(ITestOutputHelper output) : BaseExpressionConverterTests(output)
{
    [Fact]
    public async Task Test_Equals()
    {
        await ResetAndPrepareDatabase();
        var entities = ArticleRepository.Read().ToList();
        var targetId = entities[3].Id;

        var filter = new BcSerializableQuery
        {
            Page     = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType     = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = nameof(ArticleDto.Id),
                PropertyType       = typeof(Guid).FullName!,
                FilterAction       = BcGuidExpressionConverter.Equals,
                PropertyValue      = targetId.ToString()
            }
        };

        var data = await RunFilterAsync(filter, false);
        Assert.Single(data);
        Assert.Equal(targetId, data[0].Id);
    }

    [Fact]
    public async Task Test_NotEquals()
    {
        await ResetAndPrepareDatabase();
        var entities = ArticleRepository.Read().ToList();
        var targetId = entities[3].Id;

        var filter = new BcSerializableQuery
        {
            Page     = 1,
            PageSize = int.MaxValue,
            SerializableQuery = new BcSerializableQueryExpression
            {
                ExpressionType     = BcSerializableQueryExpressionType.FilterExpression,
                PropertyExpression = nameof(ArticleDto.Id),
                PropertyType       = typeof(Guid).FullName!,
                FilterAction       = BcGuidExpressionConverter.NotEquals,
                PropertyValue      = targetId.ToString()
            }
        };

        var data = await RunFilterAsync(filter, false);
        Assert.Equal(9, data.Count);
        Assert.DoesNotContain(data, dto => dto.Id == targetId);
    }
}