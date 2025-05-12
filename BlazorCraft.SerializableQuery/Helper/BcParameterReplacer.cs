using System.Linq.Expressions;

namespace BlazorCraft.SerializableQuery.Helper;

public class BcParameterReplacer(ParameterExpression from, ParameterExpression to) : ExpressionVisitor
{
    protected override Expression VisitParameter(ParameterExpression node)
        => node == from ? to : base.VisitParameter(node);
}