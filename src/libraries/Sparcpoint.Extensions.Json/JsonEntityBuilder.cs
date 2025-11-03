using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Sparcpoint.Extensions.Json;

public class JsonEntityBuilder<TEntity>
{
    internal JsonEntityBuilder(bool includeInheritedTypes)
    {
        Actions = new();
        IncludeInheritedTypes = includeInheritedTypes;
    }

    internal List<Action<JsonTypeInfo>> Actions { get; }
    internal bool IncludeInheritedTypes { get; }

    internal void AddAction(Action<JsonTypeInfo> action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        Actions.Add((info) =>
        {
            if (info.Kind != JsonTypeInfoKind.Object)
                return;

            if (!IncludeInheritedTypes && info.Type != typeof(TEntity))
                return;

            if (IncludeInheritedTypes && !typeof(TEntity).IsAssignableFrom(info.Type))
                return;

            action(info);
        });
    }

    internal IJsonTypeInfoResolver Build()
    {
        var actions = Actions
            .Where(a => a != null)
            .ToArray();

        return new EntityTypeResolver(typeof(TEntity), actions, IncludeInheritedTypes);
    }
}

internal class EntityTypeResolver : IJsonTypeInfoResolver
{
    public EntityTypeResolver(Type entityType, IEnumerable<Action<JsonTypeInfo>> actions, bool includeInheritedTypes)
    {
        EntityType = entityType;
        Actions = actions.ToList();
        IncludeInheritedTypes = includeInheritedTypes;
    }

    public List<Action<JsonTypeInfo>> Actions { get; }
    public Type EntityType { get; }
    public bool IncludeInheritedTypes { get; }

    public JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        if (type == EntityType || (IncludeInheritedTypes && EntityType.IsAssignableFrom(type)))
        {
            return new DefaultJsonTypeInfoResolver().WithAddedModifier((info) =>
            {
                foreach (var action in Actions)
                {
                    action(info);
                }
            }).GetTypeInfo(type, options);
        }

        return null;
    }
}


