using System.Linq.Expressions;

namespace BlazorCraft.SerializableQuery.ExpressionConverter.Base;

public abstract class BcExpressionConverterBase<TProperty> : IBcExpressionConverter
{
    public Type DataType => typeof(TProperty);
    public abstract string[] AvailableExpressions { get; }

    public Expression<Func<TEntity?, bool>> GetLinqExpression<TEntity>(string propertyName, string expression, object compareValue)
    {
        if (!AvailableExpressions.Contains(expression))
            throw new ArgumentException($"Expression {expression} is not supported for boolean properties.");

        var entityExpression = Expression.Parameter(typeof(TEntity), "x");
        var property = Expression.PropertyOrField(entityExpression, propertyName);
        if (property.Type != DataType && Nullable.GetUnderlyingType(property.Type) != DataType)
            throw new InvalidOperationException($"Property {propertyName} with type {property.Type}  must be of type {DataType}.");

        var converterData = GetConverterData<TEntity>(propertyName, expression, compareValue);
        return GenerateLinqExpression<TEntity>(converterData);
    }
    
    protected abstract Expression<Func<TEntity?, bool>> GenerateLinqExpression<TEntity>(BcExpressionConverterData converterData);

    protected abstract TProperty ParseData(object compareValue);

    public BcExpressionConverterData GetConverterData<TEntity>(string propertyName, string expression, object compareValue)
    {
        ParameterExpression entityExpression = Expression.Parameter(typeof(TEntity), "x");
        MemberExpression propertyExpression = Expression.PropertyOrField(entityExpression, propertyName);

        var parsedValue = ParseData(compareValue);
        
        Expression constant = Expression.Constant(parsedValue);

        var propertyType = propertyExpression.Type;
        if (constant.Type != propertyType)
        {
            constant = Expression.Convert(constant, propertyType);
        }
        
        return new BcExpressionConverterData()
        {
            ValueExpression = constant,
            EntityExpression = entityExpression,
            PropertyExpression = propertyExpression,
            Expression = expression,
            PropertyName = propertyName
        };
    }
}