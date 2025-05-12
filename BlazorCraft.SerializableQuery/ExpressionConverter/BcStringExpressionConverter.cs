using System.Linq.Expressions;
using BlazorCraft.SerializableQuery.ExpressionConverter.Base;

namespace BlazorCraft.SerializableQuery.ExpressionConverter;

public class BcStringExpressionConverter : BcExpressionConverterBase<string>
{
    public new const string Equals = "Equals";
    public const string Contains = "Contains";
    public const string StartsWith = "StartsWith";
    public const string EndsWith = "EndsWith";
    public const string NotEquals = "NotEquals";
    public const string NotContains = "NotContains";
    public const string NotStartsWith = "NotStartsWith";
    public const string NotEndsWith = "NotEndsWith";
    
    public override string[] AvailableExpressions =>   [
        Equals, NotEquals, Contains, NotContains,
        StartsWith, NotStartsWith, EndsWith, NotEndsWith
    ];

    protected override Expression<Func<TEntity?, bool>> GenerateLinqExpression<TEntity>(
        BcExpressionConverterData converterData)
        where TEntity : default
    {
        if (converterData.ValueExpression is null)
            throw new Exception($"\"{GetType().Name}\": ValueExpression is null for \"{converterData.PropertyName}\". Please check your query.");
        
        Expression filterExpression = converterData.Expression switch
        {
            Equals => Expression.Equal(converterData.PropertyExpression, converterData.ValueExpression),
            Contains => Expression.Call(converterData.PropertyExpression, nameof(string.Contains), null,  converterData.ValueExpression),
            StartsWith => Expression.Call(converterData.PropertyExpression, nameof(string.StartsWith), null,  converterData.ValueExpression),
            EndsWith => Expression.Call(converterData.PropertyExpression, nameof(string.EndsWith), null,  converterData.ValueExpression),
            
            NotEquals => Expression.Not(Expression.Equal(converterData.PropertyExpression, converterData.ValueExpression)),
            NotContains => Expression.Not(Expression.Call(converterData.PropertyExpression, nameof(string.Contains), null,  converterData.ValueExpression)),
            NotStartsWith => Expression.Not(Expression.Call(converterData.PropertyExpression, nameof(string.StartsWith), null,  converterData.ValueExpression)),
            NotEndsWith => Expression.Not(Expression.Call(converterData.PropertyExpression, nameof(string.EndsWith), null,  converterData.ValueExpression)),
            _ => throw new ArgumentOutOfRangeException(nameof(converterData.Expression), $"Unsupported expression: {converterData.Expression}")
        };

        if (converterData.PropertyExpression.Type == typeof(bool?))
        {
            var notNull = Expression.NotEqual(converterData.PropertyExpression, Expression.Constant(null, typeof(int?)));
            filterExpression = Expression.AndAlso(notNull, filterExpression);
        }

        return Expression.Lambda<Func<TEntity?, bool>>(filterExpression, converterData.EntityExpression);
    }

    protected override string ParseData(object compareValue)
    {
        return compareValue?.ToString() ?? string.Empty;
    }
}
