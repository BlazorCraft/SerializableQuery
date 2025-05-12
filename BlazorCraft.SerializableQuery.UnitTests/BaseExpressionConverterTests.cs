using System.Text.Json;
using BlazorCraft.SerializableQuery.Result.Yaml;
using BlazorCraft.Repository.Base;
using BlazorCraft.SerializableQuery;
using BlazorCraft.SerializableQuery.UnitTests.Models;
using Xunit;

namespace BlazorCraft.SerializableQuery.UnitTests;

public abstract class BaseExpressionConverterTests(ITestOutputHelper output)
{
    public BcInMemoryRepository<ArticleDto, Guid> ArticleRepository { get; } = new();
    private BcSerializableQueryExecutor CreateExecutor() => new();
    
    protected async Task ResetAndPrepareDatabase()
    {
        var articleEntities = ArticleRepository.Read().ToList();
        foreach (var entity in articleEntities)
            await ArticleRepository.Delete(entity);

        int index = 0;
        for (int i = 0; i < 10; i++)
        {
            var articleDto = new ArticleDto
            {
                Id = Guid.NewGuid(),
                ArticleNumber = $"A{i:0000}",
                Price = (i + 1) * 1000.123m,
                StockAmount = i * 100,
                InStock = i * 100 > 0,
                Status = (i & 1) == 0
                    ? Status.Even
                    : Status.Odd,
                ArticleCultures = new List<string>
                    {
                        "de",
                        "en",
                        "it",
                        "fr"
                    }.Select(x => new ArticleCultureDto()
                    {
                        Id = Guid.NewGuid(),
                        Title = $"Title {x}",
                        Culture = x,
                        Index = index++
                    })
                    .ToList(),
                Created = DateTime.Now.Date.AddDays(-i)
            };

            var articleResult = await ArticleRepository.Create(articleDto);

            if (articleResult.IsFailure())
                throw new Exception(articleResult.ToString());
        }
    }
    
    protected async Task<List<ArticleDto>> RunFilterAsync(BcSerializableQuery filter, bool cleanDatabase = true)
    {
        if (cleanDatabase)
            await ResetAndPrepareDatabase();
        
        var query = ArticleRepository.Read();
        var executor = CreateExecutor();
        var result = executor.ResolveDataSourceRequest(filter, query);

        BcSerializableQueryYamlResult<ArticleDto> yamlResult = result;
        output.WriteLine(yamlResult.ToString());

        return result.Data;
    }
}