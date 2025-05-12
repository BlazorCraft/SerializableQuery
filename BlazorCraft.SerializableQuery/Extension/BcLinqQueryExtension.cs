using System.Linq.Expressions;
using BlazorCraft.SerializableQuery.ExpressionConverter;
using BlazorCraft.SerializableQuery.ExpressionConverter.Base;
using BlazorCraft.SerializableQuery.Expressions;
using BlazorCraft.SerializableQuery.Helper;
using BlazorCraft.SerializableQuery.Sorting;

namespace BlazorCraft.SerializableQuery.Extension;

public static class BcLinqQueryExtension
{
    public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, BcSerializableQuery dataSourceOptions)
    {
        if (dataSourceOptions.SortOptions.Count == 0)
            return source;

        var parameter = Expression.Parameter(typeof(TEntity), "x");
        bool isFirst = true;

        foreach (var sortOption in dataSourceOptions.SortOptions.Where(o => o.SortDirection != BcSerializableQuerySortDirection.None))
        {
            // Supports nested property paths like "Customer.Address.City"
            Expression propertyAccess = parameter;
            foreach (var member in sortOption.PropertyTree.Split('.'))
            {
                propertyAccess = Expression.PropertyOrField(propertyAccess, member);
            }

            // Create lambda: x => x.Price (or nested prop)
            var lambda = Expression.Lambda(propertyAccess, parameter);

            string methodName = isFirst
                ? (sortOption.SortDirection == BcSerializableQuerySortDirection.Desc ? "OrderByDescending" : "OrderBy")
                : (sortOption.SortDirection == BcSerializableQuerySortDirection.Desc ? "ThenByDescending" : "ThenBy");

            // Get appropriate OrderBy / ThenBy method and invoke it
            var method = typeof(Queryable)
                .GetMethods()
                .Single(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TEntity), propertyAccess.Type);

            source = (IQueryable<TEntity>)method.Invoke(null, new object[] { source, lambda });
            isFirst = false;
        }

        return source!;
    }


    public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> queryable, IEnumerable<IBcExpressionConverter> resolvers, BcSerializableQuery dataSourceOptions)
    {
        if (dataSourceOptions.SerializableQuery is null)
            return queryable;

        var expression = BuildExpression<TEntity>(resolvers, dataSourceOptions.SerializableQuery);
        return queryable.Where(expression);
    }

    private static Expression<Func<TEntity, bool>> BuildExpression<TEntity>(IEnumerable<IBcExpressionConverter> resolvers, BcSerializableQueryExpression bcSerializableQueryExpression)
    {
        var parameterExpression = Expression.Parameter(typeof(TEntity), "x");
        var groupExpression = BuildGroupExpression<TEntity>(resolvers, bcSerializableQueryExpression, parameterExpression);
        var queryExpression = Expression.Lambda<Func<TEntity, bool>>(groupExpression, parameterExpression);
        return queryExpression;
    }

    private static Expression BuildGroupExpression<TEntity>(
        IEnumerable<IBcExpressionConverter> resolvers,
        BcSerializableQueryExpression bcSerializableQueryExpression,
        ParameterExpression param)
    {
        return bcSerializableQueryExpression.ExpressionType switch
        {
            BcSerializableQueryExpressionType.FilterExpression
                => BuildLeafExpression<TEntity>(resolvers, bcSerializableQueryExpression, param),

            BcSerializableQueryExpressionType.FilterListExpression
                => BuildListExpression<TEntity>(resolvers, bcSerializableQueryExpression, param),

            BcSerializableQueryExpressionType.AndExpression or BcSerializableQueryExpressionType.OrExpression
                => BuildOperatorExpression<TEntity>(resolvers, bcSerializableQueryExpression, param),

            _ => throw new NotImplementedException($"Unsupported expression type: {bcSerializableQueryExpression.ExpressionType}")
        };
    }

    private static Expression BuildOperatorExpression<TEntity>(IEnumerable<IBcExpressionConverter> resolvers, BcSerializableQueryExpression bcSerializableQueryExpression,
        ParameterExpression parameterExpression)
    {
        // non-leaf: AND/OR combination
        var subExpression = bcSerializableQueryExpression.Expressions
            .Select(f => BuildGroupExpression<TEntity>(resolvers, f, parameterExpression))
            .ToList();

        if (subExpression.Count == 0)
            return Expression.Constant(true);


        var combinedExpression = subExpression.Aggregate(BinaryOperator(bcSerializableQueryExpression.ExpressionType));
        return combinedExpression;
    }

    private static Func<Expression, Expression, Expression> BinaryOperator(BcSerializableQueryExpressionType groupExpressionType)
    {
        return groupExpressionType == BcSerializableQueryExpressionType.AndExpression ? Expression.AndAlso : Expression.OrElse;
    }

    private static Expression BuildLeafExpression<TEntity>(IEnumerable<IBcExpressionConverter> resolvers, BcSerializableQueryExpression bcSerializableQueryExpression,
        ParameterExpression parameterExpression)
    {
        if (bcSerializableQueryExpression.PropertyExpression is null || bcSerializableQueryExpression.PropertyValue is null || bcSerializableQueryExpression.FilterAction is null)
            throw new InvalidOperationException(
                $"""Invalid filter: "{bcSerializableQueryExpression.PropertyExpression}" "{bcSerializableQueryExpression.PropertyValue}" "{bcSerializableQueryExpression.FilterAction}".""");

        var resolver = resolvers.FirstOrDefault(r => r.DataType.FullName == bcSerializableQueryExpression.PropertyType);
        if (resolver == null)
            throw new InvalidOperationException($"No resolver for type '{bcSerializableQueryExpression.PropertyType}'.");

        // get a LambdaExpression from the resolver
        var lambda = resolver.GetLinqExpression<TEntity>(
            bcSerializableQueryExpression.PropertyExpression,
            bcSerializableQueryExpression.FilterAction,
            bcSerializableQueryExpression.PropertyValue);

        // replace the lambda's own parameter with our single 'param'
        var replaced = new BcParameterReplacer(lambda.Parameters[0], parameterExpression).Visit(lambda.Body);
        return replaced;
    }

    private static Expression BuildListExpression<TEntity>(IEnumerable<IBcExpressionConverter> resolvers, BcSerializableQueryExpression queryExpression, ParameterExpression param)
    {
        var listResolver = resolvers
                               .OfType<BcListExpressionConverter>()
                               .FirstOrDefault()
                           ?? throw new InvalidOperationException("No list-type expression converter registered.");

        // x.Tags (oder List<E>.Property)
        var listProp = Expression.PropertyOrField(param, queryExpression.PropertyExpression!);

        // Erzeuge die Nested-Lambdas für Elemente wie bisher
        var elementType = listProp.Type.GetGenericArguments()[0];
        var nestedLambdas = queryExpression.Expressions
            .Select(e =>
            {
                var subResolver = resolvers
                                      .FirstOrDefault(r => r.DataType.FullName == e.PropertyType)
                                  ?? throw new InvalidOperationException($"No resolver for type '{e.PropertyType}'.");
                var getLinq = subResolver.GetType()
                    .GetMethod(nameof(IBcExpressionConverter.GetLinqExpression))!
                    .MakeGenericMethod(elementType);

                return (LambdaExpression)getLinq.Invoke(
                    subResolver,
                    new object[] { e.PropertyExpression!, e.FilterAction!, e.PropertyValue! }
                )!;
            })
            .ToList();

        // Baue das ConverterData-Objekt
        var converterData = new BcExpressionConverterData
        {
            EntityExpression = param,
            PropertyExpression = listProp,
            Expression = queryExpression.FilterAction!, // z.B. "Any", "All" oder "Contains"
            NestedExpressions = nestedLambdas,
            PropertyName = queryExpression.PropertyExpression ?? "unknown",
            ValueExpression = default
        };

        // Rufe den List-Converter ab – das gibt uns eine Expression<Func<TEntity,bool>>
        var listLambda = listResolver.GetLinqExpression<TEntity>(queryExpression.PropertyExpression!, queryExpression.FilterAction!, converterData);

        // **Hier** extrahieren wir den Body und ersetzen den inneren Parameter durch unser 'param'
        var replacedBody = new BcParameterReplacer(
            listLambda.Parameters[0],
            param
        ).Visit(listLambda.Body);

        return replacedBody;
    }
}