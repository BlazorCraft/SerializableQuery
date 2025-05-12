namespace BlazorCraft.SerializableQuery.Expressions;

public class BcSerializableQueryExpression
{
    public Guid Id = Guid.NewGuid();
    public List<BcSerializableQueryExpression> Expressions { get; set; } = [];
    public BcSerializableQueryExpressionType ExpressionType { get; set; } = BcSerializableQueryExpressionType.AndExpression;
    public string? FilterAction { get; set; }
    public string? PropertyType { get; set; }
    public string? PropertyValue { get; set; }
    public string? PropertyExpression { get; set; }
}