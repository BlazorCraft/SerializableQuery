using System.Linq.Expressions;
using BlazorCraft.SerializableQuery.ExpressionConverter.Base;

namespace BlazorCraft.SerializableQuery.ExpressionConverter;

public class BcEnumExpressionConverter : IBcExpressionConverter
{
    public Type DataType => typeof(Enum);

    public const string Equals = "Equals";
    public const string NotEquals = "NotEquals";

    public string[] AvailableExpressions =>
    [
        Equals, NotEquals
    ];
    
    public Expression<Func<TEntity?, bool>> GetLinqExpression<TEntity>(
        string propertyName,
        string expression,
        object compareValue)
    {
        // Parameter x
        var param = Expression.Parameter(typeof(TEntity), "x");
        // x.Property
        var prop = Expression.PropertyOrField(param, propertyName);

        // Ermittle das Enum-Type (bzw. underlying für Nullable<>)
        var enumType = prop.Type;
        var isNullable = false;
        if (enumType.IsGenericType
            && enumType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            enumType = Nullable.GetUnderlyingType(enumType)!;
            isNullable = true;
        }

        if (!enumType.IsEnum)
            throw new InvalidOperationException(
                $"Property '{propertyName}' ist kein Enum- oder Nullable<Enum>-Typ.");

        // Parse den String-Namen (ignore case)
        if (compareValue == null)
            throw new ArgumentNullException(nameof(compareValue));
        var name = compareValue.ToString()!;
        var parsed = Enum.Parse(enumType, name, ignoreCase: true);

        // Baue das constante Enum-Wert-Expression
        var constType = isNullable
            ? typeof(Nullable<>).MakeGenericType(enumType)
            : enumType;
        var constant = Expression.Constant(parsed, constType);

        // Erzeuge den Vergleichs-Body
        Expression body = expression switch
        {
            Equals => Expression.Equal(prop, constant),
            NotEquals => Expression.NotEqual(prop, constant),
            _ => throw new ArgumentOutOfRangeException(
                nameof(expression),
                $"Unsupported enum expression: {expression}")
        };

        // Bei Nullable<Enum>: zusätzlich auf HasValue prüfen
        if (isNullable)
        {
            var hasValue = Expression.Property(prop, "HasValue");
            body = Expression.AndAlso(hasValue, body);
        }

        return Expression.Lambda<Func<TEntity, bool>>(body, param)!;
    }
}