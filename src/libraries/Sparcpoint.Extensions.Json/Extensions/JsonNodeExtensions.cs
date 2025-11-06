using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace System.Text.Json.Nodes;

/// <summary>
/// Provides extension methods for <see cref="JsonNode"/> objects.
/// </summary>
public static class JsonNodeExtensions
{
    /// <summary>
    /// Normalizes the JSON node by sorting properties and canonicalizing values.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static JsonNode Normalize(this JsonNode node)
    {
        switch (node)
        {
            case JsonObject obj:
                {
                    var sorted = new JsonObject();
                    foreach (var kv in obj.OrderBy(k => k.Key, StringComparer.Ordinal))
                    {
                        // DROP null-valued properties
                        if (kv.Value is null)
                            continue;

                        var child = Normalize(kv.Value);
                        sorted[kv.Key] = child;
                    }
                    return sorted;
                }

            case JsonArray arr:
                {
                    var newArr = new JsonArray();
                    foreach (var item in arr)
                    {
                        // Keep array nulls; remove if you prefer: if (item is null) continue;
                        newArr.Add(item is null ? null : Normalize(item));
                    }
                    return newArr;
                }

            case JsonValue val:
                {
                    if (val.TryGetValue<string>(out var s) &&
                        TryCanonicalizeDateTimeOffset(s, out var canon))
                    {
                        return JsonValue.Create(canon)!;
                    }
                    // Always return a fresh node to avoid "node already has a parent"
                    return val.DeepClone();
                }

            default:
                return node.DeepClone();
        }
    }

    private static bool TryCanonicalizeDateTimeOffset(
        string s,
        out string canonical)
    {
        // Liberal parse (ISO-8601 variants)
        if (!DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind, out var dto))
        {
            canonical = default!;
            return false;
        }

        // Convert as requested (UTC or preserve original offset)
        dto = dto.ToUniversalTime();

        // Quantize precision by TRUNCATION (predictable)
        long quantumTicks = 10_000;

        var ticks = dto.Ticks - (dto.Ticks % quantumTicks);
        dto = new DateTimeOffset(ticks, dto.Offset);

        // Stable output
        canonical = dto.ToString("O", CultureInfo.InvariantCulture);
        return true;
    }
}
