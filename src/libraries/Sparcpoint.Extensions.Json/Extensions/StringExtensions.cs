namespace System.Text.Json.Nodes;

/// <summary>
/// Provides extension methods for string manipulation related to JSON data.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Converts a JSON string into a canonical form, ensuring consistent ordering and formatting.
    /// </summary>
    /// <param name="json">The JSON string to be canonicalized. Cannot be null or empty.</param>
    /// <returns>A canonicalized JSON string with a compact and stable output format.</returns>
    public static string CanonicalizeJson(this string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON string cannot be null or empty.", nameof(json));

        var node = JsonNode.Parse(json)!;
        var normalized = node.Normalize();

        // Compact, stable output
        return normalized.ToJsonString(new JsonSerializerOptions { WriteIndented = false });
    }
}
