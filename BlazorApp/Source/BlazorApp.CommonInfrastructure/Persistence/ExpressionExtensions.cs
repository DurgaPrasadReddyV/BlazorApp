using System.Linq.Expressions;

namespace BlazorApp.CommonInfrastructure.Persistence;

public static class ExpressionExtensions
{
    public static string AsPath(this LambdaExpression expression)
    {
        if (expression == null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        TryParsePath(expression.Body, out string path);

        return path;
    }

    private static bool TryParsePath(Expression expression, out string path)
    {
        var noConvertExp = RemoveConvertOperations(expression);
        path = default!;

        switch (noConvertExp)
        {
            case MemberExpression memberExpression:
                {
                    string currentPart = memberExpression.Member.Name;

                    if (!TryParsePath(memberExpression.Expression!, out string parentPart))
                        return false;

                    path = string.IsNullOrEmpty(parentPart) ? currentPart : string.Concat(parentPart, ".", currentPart);
                    break;
                }

            case MethodCallExpression callExpression:
                switch (callExpression.Method.Name)
                {
                    case nameof(Queryable.Select) when callExpression.Arguments.Count == 2:
                        if (!TryParsePath(callExpression.Arguments[0], out string parentPart))
                            return false;

                        if (string.IsNullOrEmpty(parentPart))
                            return false;

                        if (callExpression.Arguments[1] is not LambdaExpression subExpression)
                            return false;

                        if (!TryParsePath(subExpression.Body, out string currentPart))
                            return false;

                        if (string.IsNullOrEmpty(parentPart))
                            return false;

                        path = string.Concat(parentPart, ".", currentPart);
                        return true;
                    case nameof(Queryable.Where):
                        throw new NotSupportedException("Filtering an Include expression is not supported");
                    case nameof(Queryable.OrderBy):
                    case nameof(Queryable.OrderByDescending):
                        throw new NotSupportedException("Ordering an Include expression is not supported");
                    default:
                        return false;
                }
        }

        return true;
    }

    private static Expression RemoveConvertOperations(Expression expression)
    {
        while (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked)
            expression = ((UnaryExpression)expression).Operand;

        return expression;
    }
}
