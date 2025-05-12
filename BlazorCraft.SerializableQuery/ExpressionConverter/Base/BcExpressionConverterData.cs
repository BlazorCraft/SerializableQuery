using System.Linq.Expressions;

namespace BlazorCraft.SerializableQuery.ExpressionConverter.Base;

public class BcExpressionConverterData
{
    public required string PropertyName { get; set; }
    public required string Expression { get; set; }
    public required ParameterExpression EntityExpression  { get; set; }
    public required MemberExpression PropertyExpression { get; set; }
    public required Expression ValueExpression { get; set; }
    public List<LambdaExpression>? NestedExpressions { get; set; }
}