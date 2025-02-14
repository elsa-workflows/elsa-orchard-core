using System;
using System.Linq.Expressions;
using System.Reflection;

namespace OrchardCore.ElsaWorkflows.Extensions;

public static class ExpressionConverter
{
    public static Expression<Func<TTarget, object>> Convert<TSource, TTarget, TProp>(
        Expression<Func<TSource, TProp>> sourceExpression)
    {
        var parameter = Expression.Parameter(typeof(TTarget), sourceExpression.Parameters[0].Name);
        var body = new PropertyReplacer(typeof(TTarget), sourceExpression.Parameters[0], parameter)
            .Visit(sourceExpression.Body);

        var convertedBody = Expression.Convert(body, typeof(object)); // Ensure return type is object.

        return Expression.Lambda<Func<TTarget, object>>(convertedBody, parameter);
    }

    private class PropertyReplacer(Type targetType, ParameterExpression sourceParameter, ParameterExpression targetParameter)
        : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == sourceParameter ? targetParameter : base.VisitParameter(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression == sourceParameter)
            {
                // Find matching property in the target type
                var targetProperty = targetType.GetProperty(node.Member.Name);
                if (targetProperty == null)
                    throw new InvalidOperationException($"Property '{node.Member.Name}' not found on target type '{targetType.Name}'");

                return Expression.Property(targetParameter, targetProperty);
            }

            return base.VisitMember(node);
        }
    }
}