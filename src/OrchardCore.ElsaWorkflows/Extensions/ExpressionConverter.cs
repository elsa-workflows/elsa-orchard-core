using System;
using System.Linq.Expressions;
using System.Reflection;

public static class ExpressionConverter
{
    public static Expression<Func<TTarget, object>> Convert<TSource, TTarget, TProp>(
        Expression<Func<TSource, TProp>> sourceExpression)
    {
        var parameter = Expression.Parameter(typeof(TTarget), sourceExpression.Parameters[0].Name);
        var body = new PropertyReplacer(typeof(TSource), typeof(TTarget), sourceExpression.Parameters[0], parameter)
            .Visit(sourceExpression.Body);

        var convertedBody = Expression.Convert(body, typeof(object)); // Ensure return type is object

        return Expression.Lambda<Func<TTarget, object>>(convertedBody, parameter);
    }

    private class PropertyReplacer : ExpressionVisitor
    {
        private readonly Type _sourceType;
        private readonly Type _targetType;
        private readonly ParameterExpression _sourceParameter;
        private readonly ParameterExpression _targetParameter;

        public PropertyReplacer(Type sourceType, Type targetType, ParameterExpression sourceParameter, ParameterExpression targetParameter)
        {
            _sourceType = sourceType;
            _targetType = targetType;
            _sourceParameter = sourceParameter;
            _targetParameter = targetParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _sourceParameter ? _targetParameter : base.VisitParameter(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression == _sourceParameter)
            {
                // Find matching property in the target type
                PropertyInfo? targetProperty = _targetType.GetProperty(node.Member.Name);
                if (targetProperty == null)
                {
                    throw new InvalidOperationException($"Property '{node.Member.Name}' not found on target type '{_targetType.Name}'");
                }

                return Expression.Property(_targetParameter, targetProperty);
            }

            return base.VisitMember(node);
        }
    }
}
