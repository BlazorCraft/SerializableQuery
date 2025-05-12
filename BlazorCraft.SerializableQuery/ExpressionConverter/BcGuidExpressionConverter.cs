using System.Linq.Expressions;
using BlazorCraft.SerializableQuery.ExpressionConverter.Base;

namespace BlazorCraft.SerializableQuery.ExpressionConverter;

public class BcGuidExpressionConverter : BcExpressionConverterBase<Guid>
{
    public new const string Equals    = "Equals";
    public const string NotEquals     = "NotEquals";

    public override string[] AvailableExpressions => [Equals, NotEquals];

    protected override Expression<Func<TEntity?, bool>> GenerateLinqExpression<TEntity>(
        BcExpressionConverterData converterData)
        where TEntity : default
    {
        Expression filterExpression = converterData.Expression switch
        {
            Equals    => Expression.Equal(converterData.PropertyExpression, converterData.ValueExpression),
            NotEquals => Expression.NotEqual(converterData.PropertyExpression, converterData.ValueExpression),
            _         => throw new ArgumentOutOfRangeException(
                nameof(converterData.Expression),
                $"Unsupported expression: {converterData.Expression}")
        };

        return Expression.Lambda<Func<TEntity?, bool>>(filterExpression, converterData.EntityExpression);
    }

    protected override Guid ParseData(object compareValue)
    {
        if (compareValue is Guid g)
            return g;

        if (compareValue is string s && Guid.TryParse(s, out var parsed))
            return parsed;

        throw new ArgumentException($"Invalid Guid value: {compareValue}");
    }
}