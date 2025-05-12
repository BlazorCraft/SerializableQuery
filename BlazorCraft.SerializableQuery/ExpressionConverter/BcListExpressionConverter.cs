using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using BlazorCraft.SerializableQuery.ExpressionConverter.Base;
using BlazorCraft.SerializableQuery.Helper;

namespace BlazorCraft.SerializableQuery.ExpressionConverter;

public class BcListExpressionConverter : IBcExpressionConverter
{
    public Type DataType => typeof(IEnumerable);

    public const string Any = "Any";
    public const string All = "All";
    public const string Contains = "Contains";
    public string[] AvailableExpressions => [Any, All, Contains];

    public Expression<Func<TEntity?, bool>> GetLinqExpression<TEntity>(string propertyName, string expression, object compareValue)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var listProperty = Expression.PropertyOrField(parameter, propertyName);

        if (!typeof(IEnumerable).IsAssignableFrom(listProperty.Type))
            throw new InvalidOperationException(
                $"Property '{propertyName}' is not a list or enumerable.");

        Type elementType = listProperty.Type.IsGenericType
            ? listProperty.Type.GetGenericArguments()[0]
            : typeof(object);

        MethodInfo method;
        Expression body;

        switch (expression)
        {
            case Any:
            case All:
                if (compareValue is not BcExpressionConverterData converterData || converterData.NestedExpressions == null)
                {
                    throw new ArgumentException($"Invalid converter data for '{expression}' on list '{propertyName}'.");
                }

                var elemParam = Expression.Parameter(elementType, "e");

                Expression? combined = null;
                foreach (var lambda in converterData.NestedExpressions)
                {
                    var replaced = new BcParameterReplacer(lambda.Parameters[0], elemParam)
                        .Visit(lambda.Body);
                    if (combined == null)
                        combined = replaced;
                    else
                        combined = expression == Any
                            ? Expression.OrElse(combined, replaced)
                            : Expression.AndAlso(combined, replaced);
                }

                combined ??= Expression.Constant(true);

                method = typeof(Enumerable)
                    .GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .First(m => m.Name == expression && m.GetParameters().Length == 2)
                    .MakeGenericMethod(elementType);

                body = Expression.Call(method, listProperty,
                    Expression.Lambda(combined, elemParam));
                break;

            case Contains:
                var constant = Expression.Constant(compareValue, elementType);

                method = typeof(Enumerable)
                    .GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .First(m => m.Name == Contains && m.GetParameters().Length == 2)
                    .MakeGenericMethod(elementType);

                body = Expression.Call(method, listProperty, constant);
                break;

            default:
                throw new NotSupportedException(
                    $"List-Filter '{expression}' is not supported.");
        }


        return Expression.Lambda<Func<TEntity, bool>>(body, parameter)!;
    }
}