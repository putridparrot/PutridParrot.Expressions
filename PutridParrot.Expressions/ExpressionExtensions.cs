using System.Linq.Expressions;
using System.Reflection;

namespace PutridParrot.Expressions;

public static class ExpressionExtensions
{
    /// <summary>
    /// Extracts the instance object from a function expressions
    /// </summary>
    /// <typeparam name="TRet"></typeparam>
    /// <param name="expression"></param>
    /// <returns>The instance the function is called on</returns>
    /// <exception cref="Exception"></exception>
    public static object InstanceOf<TRet>(this Expression<Func<TRet>> expression)
    {
        var method = expression.Body as MethodCallExpression;
        var memberExpression = method == null
            ? expression.Body as MemberExpression
            : method.Object as MemberExpression;

        if (memberExpression == null)
            throw new Exception(
                "Unable to determine expression type. Expected () => vm.Property or () => vm.DoSomething()");

        while (memberExpression.Expression is MemberExpression)
            memberExpression = memberExpression.Expression as MemberExpression;

        var constantExpression = memberExpression.Expression as ConstantExpression;
        if (constantExpression == null)
            throw new Exception("Cannot determine constant expression");

        var fieldInfo = memberExpression.Member as FieldInfo;
        if (fieldInfo == null)
            throw new Exception("Cannot determine fieldinfo object");

        return fieldInfo.GetValue(constantExpression.Value);
    }

    public static string NameOf<T>(Expression<Func<T>> propertyExpression)
    {
        if (propertyExpression == null)
            throw new ArgumentNullException(nameof(propertyExpression));

        MemberExpression property = null;

        if (propertyExpression.Body.NodeType == ExpressionType.Convert)
        {
            if (propertyExpression.Body is UnaryExpression convert)
            {
                property = convert.Operand as MemberExpression;
            }
        }

        if (property == null)
        {
            property = propertyExpression.Body as MemberExpression;
        }

        if (property == null)
        {
            throw new Exception(
                "propertyExpression cannot be null and should be passed in the format x => x.PropertyName");
        }

        return property.Member.Name;
    }

    public static string[] NameOf<T>(params Expression<Func<T>>[] propertyExpressions)
    {
        return propertyExpressions.Select(NameOf).ToArray();
    }

    public static string NameOf<T>(Expression<Func<T, object>> expression)
    {
        return expression.ToMemberInfo().Name;
    }

    public static string[] NameOf<T>(IEnumerable<Expression<Func<T, object>>> expressions)
    {
        return expressions.Select(e => e.ToMemberInfo().Name).ToArray();
    }

    public static MemberInfo ToMemberInfo(this Expression expression)
    {
        MemberExpression memberExpression = null;
        if (expression is LambdaExpression lambdaExpression)
        {
            memberExpression = lambdaExpression.Body is UnaryExpression unaryExpression
                ? unaryExpression.Operand as MemberExpression
                : lambdaExpression.Body as MemberExpression;
        }
        return memberExpression?.Member;
    }
}