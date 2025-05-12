using BlazorCraft.Repository.Interfaces;

namespace BlazorCraft.SerializableQuery.UnitTests.Models;

public class ArticleDto :  IBcEntity<Guid>
{
    public Guid Id { get; set; }
    public required DateTime Created { get; set; }
    public bool InStock { get; set; }
    public int StockAmount { get; set; }
    public required string ArticleNumber { get; set; }
    public decimal? Price { get; set; }
    public Status Status { get; set; }
    public List<ArticleCultureDto> ArticleCultures { get; set; } = [];
}