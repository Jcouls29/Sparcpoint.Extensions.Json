using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization.Metadata;

namespace Sparcpoint.Extensions.Json;

internal static class JsonTypeInfoExtensions
{
    public static JsonPropertyInfo? FindProperty<TEntity>(this JsonTypeInfo info, MemberInfo member)
        => info.Properties.FirstOrDefault(p => (p.AttributeProvider as MemberInfo) == member);
}
