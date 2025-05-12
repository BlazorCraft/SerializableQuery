using System.Linq.Expressions;

namespace BlazorCraft.SerializableQuery.ExpressionConverter.Base;

public interface IBcExpressionConverter
{
    public Type DataType { get; }
    public string[] AvailableExpressions { get; }
    public Expression<Func<TEntity?, bool>> GetLinqExpression<TEntity>(string propertyName, string expression, object compareValue);
}