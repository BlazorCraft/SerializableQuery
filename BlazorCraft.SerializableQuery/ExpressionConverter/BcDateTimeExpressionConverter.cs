using System.Linq.Expressions;
using BlazorCraft.SerializableQuery.ExpressionConverter.Base;

namespace BlazorCraft.SerializableQuery.ExpressionConverter;

public class BcDateTimeExpressionConverter : BcExpressionConverterBase<DateTime>
{
    public new const string Equals = "Equals";
    public const string NotEquals = "NotEquals";
    public const string Greater = "Greater";
    public const string Lower = "Lower";
    
    public override string[] AvailableExpressions =>  [Equals, NotEquals, Greater, Lower];

    protected override Expression<Func<TEntity?, bool>> GenerateLinqExpression<TEntity>(BcExpressionConverterData converterData)
        where TEntity : default
    {
   
        
        Expression filterExpression = converterData.Expression switch
        {
            Equals => Expression.Equal(converterData.PropertyExpression, converterData.ValueExpression),
            NotEquals => Expression.NotEqual(converterData.PropertyExpression, converterData.ValueExpression),
            Greater => Expression.GreaterThan(converterData.PropertyExpression, converterData.ValueExpression),
            Lower => Expression.LessThan(converterData.PropertyExpression, converterData.ValueExpression),
            _ => throw new ArgumentOutOfRangeException(nameof(converterData.Expression), $"Unsupported expression: {converterData.Expression}")
        };

        // Add null-check if the property is nullable
        if (converterData.PropertyExpression.Type == typeof(DateTime?))
        {
            var notNull = Expression.NotEqual(converterData.PropertyExpression, Expression.Constant(null, typeof(DateTime?)));
            filterExpression = Expression.AndAlso(notNull, filterExpression);
        }

        return Expression.Lambda<Func<TEntity?, bool>>(filterExpression, converterData.EntityExpression);
    }

    protected override DateTime ParseData(object compareValue)
    {
        if(compareValue is DateTime value)
            return value;
        
        if (!DateTime.TryParse(compareValue.ToString(), out var parsedValue))
            throw new ArgumentException($"Invalid DateTime value: {compareValue}");

        return parsedValue;
    }
}