using Sparcpoint.Extensions.Json;
using System.Collections;
using System.Linq;
using System.Text.Json.Serialization.Metadata;

namespace System.Text.Json;

public static class JsonSerializerOptionsExtensions
{
    public static JsonSerializerOptions WithLowerPriority<T>(this JsonSerializerOptions options, Action<JsonEntityBuilder<T>> configure, bool includeInheritedTypes = true)
    {
        if (!IsValidType(typeof(T)))
            throw new InvalidOperationException("Only classes and structs can be configured.");

        var builder = new JsonEntityBuilder<T>(includeInheritedTypes);
        configure(builder);
        options.TypeInfoResolverChain.Add(builder.Build());

        CheckLastInChainIsDefault(options);

        return options;
    }

    public static JsonSerializerOptions WithHigherPriority<T>(this JsonSerializerOptions options, Action<JsonEntityBuilder<T>> configure, bool includeInheritedTypes = true)
    {
        if (!IsValidType(typeof(T)))
            throw new InvalidOperationException("Only classes and structs can be configured.");

        var builder = new JsonEntityBuilder<T>(includeInheritedTypes);
        configure(builder);
        options.TypeInfoResolverChain.Insert(0, builder.Build());

        CheckLastInChainIsDefault(options);

        return options;
    }

    private static void CheckLastInChainIsDefault(this JsonSerializerOptions options)
    {
        var newChain = options.TypeInfoResolverChain.Where(t => !(t is DefaultJsonTypeInfoResolver resolver && !resolver.Modifiers.Any())).ToArray();
        options.TypeInfoResolverChain.Clear();

        foreach(var link in newChain)
        {
            options.TypeInfoResolverChain.Add(link);
        }
        options.TypeInfoResolverChain.Add(new DefaultJsonTypeInfoResolver());
    }

    private static bool IsValidType(this Type type)
    {
        return
            (type.IsClass || type.IsValueType) &&
            !type.IsPrimitive &&
            !type.IsEnum &&
            !type.IsArray &&
            !typeof(IEnumerable).IsAssignableFrom(type) &&
            !IsSystemType(type)
        ;
    }

    private static bool IsSystemType(this Type t)
    {
        // Unwrap nullable
        t = Nullable.GetUnderlyingType(t) ?? t;

        // Unwrap array / pointer / byref
        if (t.HasElementType)
            return IsSystemType(t.GetElementType()!);

        // Handle generics: treat as "System" if the generic itself or *all* args are System
        if (t.IsGenericType)
        {
            if (IsSystemNamespace(t))
                return true;
            foreach (var ga in t.GetGenericArguments())
                if (!IsSystemType(ga))
                    return false;
            return true;
        }

        return IsSystemNamespace(t);
    }

    private static bool IsSystemNamespace(Type t)
        => t.Namespace is string ns &&
           (ns == "System" || ns.StartsWith("System.", StringComparison.Ordinal));
}
