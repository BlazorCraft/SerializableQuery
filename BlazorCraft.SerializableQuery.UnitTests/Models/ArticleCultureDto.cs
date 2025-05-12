using BlazorCraft.Repository.Interfaces;

namespace BlazorCraft.SerializableQuery.UnitTests.Models;

public class ArticleCultureDto: IBcEntity<Guid>
{
    public required Guid Id { get; set; }
    public required string Culture { get; set; }
    public required string Title { get; set; }
    public int Index { get; set; }
}