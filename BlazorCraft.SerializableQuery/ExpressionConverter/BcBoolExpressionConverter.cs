using System.Linq.Expressions;
using BlazorCraft.SerializableQuery.ExpressionConverter.Base;

namespace BlazorCraft.SerializableQuery.ExpressionConverter;

public class BcBoolExpressionConverter : BcExpressionConverterBase<bool>
{
    public new const string Equals = "Equals";
    public const string NotEquals = "NotEquals";
    
    public override string[] AvailableExpressions => [Equals, NotEquals];

    protected override Expression<Func<TEntity?, bool>> GenerateLinqExpression<TEntity>(BcExpressionConverterData converterData) where TEntity : default
    {
        Expression filterExpression = converterData.Expression switch
        {
            Equals => Expression.Equal(converterData.PropertyExpression, converterData.ValueExpression),
            NotEquals => Expression.NotEqual(converterData.PropertyExpression, converterData.ValueExpression),
            _ => throw new ArgumentOutOfRangeException(nameof(converterData.Expression), $"Unsupported action: {converterData.Expression}")
        };

        if (converterData.PropertyExpression.Type == typeof(bool?))
        {
            var notNull = Expression.NotEqual(converterData.PropertyExpression, Expression.Constant(null, typeof(int?)));
            filterExpression = Expression.AndAlso(notNull, filterExpression);
        }
        
        return Expression.Lambda<Func<TEntity?, bool>>(filterExpression, converterData.EntityExpression);
    }

    protected override bool ParseData(object compareValue)
    {
        if(compareValue is bool value)
            return value;
        
        if (!bool.TryParse(compareValue.ToString(), out var parsedValue))
            throw new ArgumentException($"Invalid boolean value: {compareValue}");

        return parsedValue;
    }
}