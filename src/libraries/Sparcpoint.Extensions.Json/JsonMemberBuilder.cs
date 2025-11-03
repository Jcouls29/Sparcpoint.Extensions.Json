using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Sparcpoint.Extensions.Json;

public class JsonMemberBuilder<TEntity, TProperty>
{
    private readonly JsonEntityBuilder<TEntity> _Builder;
    private readonly MemberInfo _Member;

    internal JsonMemberBuilder(JsonEntityBuilder<TEntity> builder, MemberInfo member)
    {
        _Builder = builder ?? throw new ArgumentNullException(nameof(builder));
        _Member = member ?? throw new ArgumentNullException(nameof(member));
    }

    public JsonMemberBuilder<TEntity, TProperty> Name(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidOperationException("Name of property cannot be empty.");

        // TODO: Validate name is valid

        return WithProperty((info, prop) =>
        {
            if (prop == null)
                return;

            prop.Name = name;
        });
    }

    public JsonMemberBuilder<TEntity, TProperty> Ignore()
        => WithProperty((info, prop) =>
        {
            if (prop == null)
                return;

            info.Properties.Remove(prop);
        });

    public JsonMemberBuilder<TEntity, TProperty> Order(int order)
        => WithProperty((info, prop) =>
        {
            if (prop == null)
                return;

            prop.Order = order;
        });

    public JsonMemberBuilder<TEntity, TProperty> Required()
        => WithProperty((info, prop) =>
        {
            if (prop == null)
                return;

            prop.IsRequired = true;
        });

    public JsonMemberBuilder<TEntity, TProperty> NumberHandling(JsonNumberHandling? handling)
        => WithProperty((info, prop) =>
        {
            if (prop == null)
                return;

            prop.NumberHandling = handling;
        });

    public JsonMemberBuilder<TEntity, TProperty> ObjectCreationHandling(JsonObjectCreationHandling? handling)
        => WithProperty((info, prop) =>
        {
            if (prop == null)
                return;

            prop.ObjectCreationHandling = handling;
        });

    public JsonMemberBuilder<TEntity, TProperty> CustomConverter(JsonConverter? converter)
        => WithProperty((info, prop) =>
        {
            if (prop == null)
                return;

            prop.CustomConverter = converter;
        });

    public JsonMemberBuilder<TEntity, TPropertyNext> Property<TPropertyNext>(Expression<Func<TEntity, TPropertyNext>> expression)
    {
        // TODO: Commonize this check
        if (expression.Body is MemberExpression memberExpr)
            return new JsonMemberBuilder<TEntity, TPropertyNext>(_Builder, memberExpr.Member);

        if (expression.Body is UnaryExpression unaryExpr && unaryExpr.Operand is MemberExpression innerMemberExpr)
            return new JsonMemberBuilder<TEntity, TPropertyNext>(_Builder, innerMemberExpr.Member);

        throw new InvalidOperationException($"Expression '{expression}' does not refer to a property or field.");
    }

    private JsonMemberBuilder<TEntity, TProperty> WithProperty(Action<JsonTypeInfo, JsonPropertyInfo?> action)
    {
        _Builder.AddAction((info) =>
        {
            action(info, info.FindProperty<TEntity>(_Member));
        });

        return this;
    }
}