using System.Globalization;
using System.Linq.Expressions;
using BlazorCraft.SerializableQuery.ExpressionConverter.Base;

namespace BlazorCraft.SerializableQuery.ExpressionConverter;

public class BcDecimalExpressionConverter : BcExpressionConverterBase<decimal>
{
    public new const string Equals = nameof(Equals);
    public const string NotEquals = "NotEquals";
    public const string GreaterThanOrEqual = nameof(GreaterThanOrEqual);
    public const string LessThanOrEqual = nameof(LessThanOrEqual);
    public const string NotGreaterThanOrEqual = "NotGreaterThanOrEqual";
    public const string NotLessThanOrEqual = "NotLessThanOrEqual";
    
    public override string[] AvailableExpressions =>   [
        Equals, NotEquals,
        GreaterThanOrEqual, LessThanOrEqual,
        NotGreaterThanOrEqual, NotLessThanOrEqual
    ];

    protected override Expression<Func<TEntity?, bool>> GenerateLinqExpression<TEntity>(BcExpressionConverterData converterData) where TEntity : default
    {
        if (converterData.ValueExpression is null)
            throw new Exception($"\"{GetType().Name}\": ValueExpression is null for \"{converterData.PropertyName}\". Please check your query.");
        
        Expression filterExpression = converterData.Expression switch
        {
            Equals => Expression.Equal(converterData.PropertyExpression, converterData.ValueExpression),
            NotEquals => Expression.NotEqual(converterData.PropertyExpression, converterData.ValueExpression),
            GreaterThanOrEqual => Expression.GreaterThanOrEqual(converterData.PropertyExpression, converterData.ValueExpression),
            LessThanOrEqual => Expression.LessThanOrEqual(converterData.PropertyExpression, converterData.ValueExpression),
            NotGreaterThanOrEqual =>
                Expression.Not(Expression.GreaterThanOrEqual(converterData.PropertyExpression, converterData.ValueExpression)),
            NotLessThanOrEqual => Expression.Not(Expression.LessThanOrEqual(converterData.PropertyExpression, converterData.ValueExpression)),
            _ => throw new ArgumentOutOfRangeException(nameof(converterData.Expression), $"Unsupported expression: {converterData.Expression}")
        };

        if (converterData.PropertyExpression.Type == typeof(decimal?))
        {
            var notNull = Expression.NotEqual(converterData.PropertyExpression, Expression.Constant(null, typeof(decimal?)));
            filterExpression = Expression.AndAlso(notNull, filterExpression);
        }

        return Expression.Lambda<Func<TEntity?, bool>>(filterExpression, converterData.EntityExpression);
    }

    protected override decimal ParseData(object compareValue)
    {
        if (compareValue is decimal value)
            return value;

        var str = compareValue?.ToString();
        if (string.IsNullOrWhiteSpace(str))
            throw new ArgumentException($"Invalid decimal value: {compareValue}");

        var sep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        str = str.Replace(",", sep).Replace(".", sep);

        if (!decimal.TryParse(str, NumberStyles.Number, CultureInfo.CurrentCulture, out var parsedValue))
            throw new ArgumentException($"Invalid decimal value: {compareValue}");

        return parsedValue;
    }
}