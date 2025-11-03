using System;
using System.Linq.Expressions;

namespace Sparcpoint.Extensions.Json;

public static class JsonEntityBuilderExtensions
{
    public static JsonMemberBuilder<TEntity, TProperty> Property<TEntity, TProperty>(this JsonEntityBuilder<TEntity> builder, Expression<Func<TEntity, TProperty>> expression)
    {
        if (expression.Body is MemberExpression memberExpr)
            return new JsonMemberBuilder<TEntity, TProperty>(builder, memberExpr.Member);

        if (expression.Body is UnaryExpression unaryExpr && unaryExpr.Operand is MemberExpression innerMemberExpr)
            return new JsonMemberBuilder<TEntity, TProperty>(builder, innerMemberExpr.Member);

        throw new InvalidOperationException($"Expression '{expression}' does not refer to a property or field.");
    }
}
