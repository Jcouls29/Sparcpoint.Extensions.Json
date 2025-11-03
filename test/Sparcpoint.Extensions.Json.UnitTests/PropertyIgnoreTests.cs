using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sparcpoint.Extensions.Json.UnitTests;

public class PropertyIgnoreTests
{
    private const string PROP_VALUE = "PropertyValue";

    [Fact]
    public void CanIgnoreProperty()
    {
        var json = Serialize((b) => { });
        AssertContains(json, nameof(SampleClass.NotIgnoredProperty));
        AssertContains(json, nameof(SampleClass.NotIgnoredField));

        json = Serialize((b) => b.Property(t => t.NotIgnoredProperty).Ignore());
        AssertNotContains(json, nameof(SampleClass.NotIgnoredProperty));
        AssertContains(json, nameof(SampleClass.NotIgnoredField));
    }

    [Fact]
    public void IgnoredPropertyStaysIgnored()
    {
        var json = Serialize((b) => { });
        AssertNotContains(json, nameof(SampleClass.IgnoredProperty));
        AssertContains(json, nameof(SampleClass.NotIgnoredProperty));
        AssertContains(json, nameof(SampleClass.NotIgnoredField));

        json = Serialize((b) => b.Property(t => t.IgnoredProperty).Ignore());
        AssertNotContains(json, nameof(SampleClass.IgnoredProperty));
        AssertContains(json, nameof(SampleClass.NotIgnoredProperty));
        AssertContains(json, nameof(SampleClass.NotIgnoredField));
    }

    [Fact]
    public void CanIgnoreField()
    {
        var json = Serialize((b) => { });
        AssertContains(json, nameof(SampleClass.NotIgnoredProperty));
        AssertContains(json, nameof(SampleClass.NotIgnoredField));

        json = Serialize((b) => b.Property(t => t.NotIgnoredField).Ignore());
        AssertNotContains(json, nameof(SampleClass.NotIgnoredField));
        AssertContains(json, nameof(SampleClass.NotIgnoredProperty));
    }

    [Fact]
    public void IgnoredFieldStaysIgnored()
    {
        var json = Serialize((b) => { });
        AssertNotContains(json, nameof(SampleClass.IgnoredField));
        AssertContains(json, nameof(SampleClass.NotIgnoredProperty));
        AssertContains(json, nameof(SampleClass.NotIgnoredField));

        json = Serialize((b) => b.Property(t => t.IgnoredField).Ignore());
        AssertNotContains(json, nameof(SampleClass.IgnoredField));
        AssertContains(json, nameof(SampleClass.NotIgnoredProperty));
        AssertContains(json, nameof(SampleClass.NotIgnoredField));
    }

    private string Serialize(Action<JsonEntityBuilder<SampleClass>>? configure = null)
    {
        var options = new JsonSerializerOptions();
        options.IncludeFields = true;
        options.Configure<SampleClass>(b => configure?.Invoke(b));

        var data = new SampleClass();

        var json = JsonSerializer.Serialize(data, options);
        Assert.NotNull(json);
        Assert.NotEmpty(json);

        return json;
    }

    private void AssertContains(string json, string name)
    {
        Assert.Contains($"\"{EncodeName(name)}\":\"{PROP_VALUE}\"", json);
    }

    private void AssertNotContains(string json, string name)
    {
        Assert.DoesNotContain($"\"{EncodeName(name)}\":\"{PROP_VALUE}\"", json);
    }

    private string EncodeName(string name)
    {
        var encodedName = JsonSerializer.Serialize(name);
        return encodedName[1..^1];  // Remove Quotes
    }

    public class SampleClass
    {
        public string NotIgnoredProperty { get; set; } = PROP_VALUE;

        [JsonIgnore]
        public string IgnoredProperty { get; set; } = PROP_VALUE;

        public string NotIgnoredField = PROP_VALUE;

        [JsonIgnore]
        public string IgnoredField = PROP_VALUE;
    }
}

