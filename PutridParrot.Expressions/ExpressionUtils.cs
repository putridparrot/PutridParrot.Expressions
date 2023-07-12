using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PutridParrot.Expressions;

public static class ExpressionUtils
{
    public static Expression ToConstExpression(object o) =>
        Expression.Constant(o);

    public static Expression ToStringExpression(object o) =>
        Expression.Constant(o?.ToString());

    public static Expression ToDictionaryCall(Expression parameter, string keyName) =>
        Expression.Call(null,
            typeof(DictionaryExtensions).GetMethod("GetValue",
                new[] { typeof(IDictionary<string, object>), typeof(string) })!,
            parameter, ToConstExpression(keyName));

    public static Expression ToConvertExpression(Expression expression, Type convertTo) =>
        Expression.Convert(expression, convertTo);

    public static Expression ToConvertExpression<TType>(Expression expression) =>
        ToConvertExpression(expression, typeof(TType));

    /// <summary>
    /// Simple way to call a static type function, i.e. String.StartsWith can be called
    /// as ToStaticFunctionExpression&lt;String&gt;("StartsWith", args) where the 
    /// </summary>
    /// <typeparam name="TStaticType"></typeparam>
    /// <typeparam name="TFirstArgType"></typeparam>
    /// <param name="functionName"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static Expression ToStaticFunctionExpression<TStaticType, TFirstArgType>(string functionName, Expression[] args) =>
        Expression.Call(ToConvertExpression<TFirstArgType>(args[0]),
            typeof(TStaticType).GetMethod(functionName, new [] {typeof(TFirstArgType)})!, args[0]);

    public static Expression ToCompareExpression(Expression left, Expression right) =>
        Expression.Call(null, typeof(ObjectComparer).GetMethod("Compare", new[] {typeof(object), typeof(object)})!,
            Expression.Convert(left, typeof(object)),
            Expression.Convert(right, typeof(object)));
}