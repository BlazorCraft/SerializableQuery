using System.Linq.Expressions;

namespace BlazorCraft.SerializableQuery.Helper;

public class BcPropertyTreeHelper<TEntity>
{
    public string GetPropertyNames(Expression? propertyExpression)
    {
        if (propertyExpression is MemberExpression parentExpression)
            return GetPropertyNames(parentExpression);

        throw new ArgumentException("The Expression is not a MemberExpression");
    }

    public string GetPropertyNames(MemberExpression? propertyExpression)
    {
        if (propertyExpression is null)
            return string.Empty;

        if (propertyExpression.Expression is null)
            return propertyExpression.Member.Name;

        if (propertyExpression.Expression is MemberExpression childExpression)
            return $"{GetPropertyNames(childExpression)}.{propertyExpression.Member.Name}";

        return propertyExpression.Member.Name;
    }

    public string GetPropertyTree<TProperty>(Expression<Func<TEntity, TProperty>> expression)
    {
        return GetPropertyNames(expression.Body);
    }

    public string GetPropertyTree<TListProperty, TProperty>(
        Expression<Func<TEntity, List<TListProperty>>> listExpressionL1,
        Expression<Func<TListProperty, TProperty>> valueExpression
    )
    {
        return string.Join(".", [
            GetPropertyNames(listExpressionL1.Body),
            GetPropertyNames(valueExpression)
        ]);
    }

    public string GetPropertyTree<TListProperty1, TListProperty2, TProperty>(
        Expression<Func<TEntity, List<TListProperty1>>> listExpressionL1,
        Expression<Func<TListProperty1, TListProperty2>> listExpressionL2,
        Expression<Func<TListProperty2, TProperty>> valueExpression
    )
    {
        return string.Join(".", [
            GetPropertyNames(listExpressionL1.Body),
            GetPropertyNames(listExpressionL2.Body),
            GetPropertyNames(valueExpression)
        ]);
    }

    public string GetPropertyTree<TListProperty1, TListProperty2, TListProperty3, TProperty>(
        Expression<Func<TEntity, List<TListProperty1>>> listExpressionL1,
        Expression<Func<TListProperty1, TListProperty2>> listExpressionL2,
        Expression<Func<TListProperty2, TListProperty3>> listExpressionL3,
        Expression<Func<TListProperty3, TProperty>> valueExpression
    )
    {
        return string.Join(".", [
            GetPropertyNames(listExpressionL1.Body),
            GetPropertyNames(listExpressionL2.Body),
            GetPropertyNames(listExpressionL3.Body),
            GetPropertyNames(valueExpression)
        ]);
    }

    public string GetPropertyTree<TListProperty1, TListProperty2, TListProperty3, TListProperty4, TProperty>(
        Expression<Func<TEntity, List<TListProperty1>>> listExpressionL1,
        Expression<Func<TListProperty1, TListProperty2>> listExpressionL2,
        Expression<Func<TListProperty2, TListProperty3>> listExpressionL3,
        Expression<Func<TListProperty2, TListProperty4>> listExpressionL4,
        Expression<Func<TListProperty4, TProperty>> valueExpression
    )
    {
        return string.Join(".", [
            GetPropertyNames(listExpressionL1.Body),
            GetPropertyNames(listExpressionL2.Body),
            GetPropertyNames(listExpressionL3.Body),
            GetPropertyNames(listExpressionL4.Body),
            GetPropertyNames(valueExpression)
        ]);
    }
}