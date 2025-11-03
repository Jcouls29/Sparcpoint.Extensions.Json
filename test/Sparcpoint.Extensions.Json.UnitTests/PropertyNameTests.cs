using System.Text.Json;
using System.Xml.Linq;

namespace Sparcpoint.Extensions.Json.UnitTests;

public class PropertyNameTests
{
    private const string PROP_VALUE = "PropertyValue";

    [Fact]
    public void ConfirmNaturalSerialization()
    {
        var json = Serialize();
        Assert.Contains($"\"{nameof(SampleClass.Property)}\":\"{PROP_VALUE}\"", json);
        Assert.Contains($"\"{nameof(SampleClass.Field)}\":\"{PROP_VALUE}\"", json);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void InvalidNamesThrows(string? name)
    {
        Assert.Throws<InvalidOperationException>(() => Serialize((b) => b.Property(t => t.Property).Name(name)));
        Assert.Throws<InvalidOperationException>(() => Serialize((b) => b.Property(t => t.Field).Name(name)));
    }

    [Theory]
    [InlineData("PascalCase")]
    [InlineData("camelCase")]
    [InlineData("snake_case")]
    [InlineData("kebob-case")]
    [InlineData("_-_-_")]
    [InlineData("''``\"")]
    [InlineData("With Spaces")]
    public void CanChangePropertyNames(string name)
    {
        var json = Serialize((b) => b.Property(t => t.Property).Name(name));
        Assert.Contains($"\"{EncodeName(name)}\":\"{PROP_VALUE}\"", json);
        Assert.Contains($"\"{nameof(SampleClass.Field)}\":\"{PROP_VALUE}\"", json);
    }

    [Theory]
    [InlineData("PascalCase")]
    [InlineData("camelCase")]
    [InlineData("snake_case")]
    [InlineData("kebob-case")]
    [InlineData("_-_-_")]
    [InlineData("''``\"")]
    [InlineData("With Spaces")]
    public void CanChangeFieldNames(string name)
    {
        var json = Serialize((b) => b.Property(t => t.Field).Name(name));
        Assert.Contains($"\"{EncodeName(name)}\":\"{PROP_VALUE}\"", json);
        Assert.Contains($"\"{nameof(SampleClass.Property)}\":\"{PROP_VALUE}\"", json);
    }

    [Fact]
    public void CanChangeMultipleMemberNames()
    {
        var json = Serialize((b) => b
            .Property(t => t.Property).Name("prop-changed")
            .Property(t => t.Field).Name("field-changed")
        );
        Assert.Contains($"\"prop-changed\":\"{PROP_VALUE}\"", json);
        Assert.Contains($"\"field-changed\":\"{PROP_VALUE}\"", json);
    }

    [Fact]
    public void ThrowsOnSameNames()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            Serialize((b) => b
                .Property(t => t.Property).Name("same-name")
                .Property(t => t.Field).Name("same-name")
            );
        });
    }

    [Fact]
    public void LastNameChangeWins()
    {
        var json = Serialize((b) => b
            .Property(t => t.Property).Name("name-01")
            .Property(t => t.Property).Name("name-02")
            .Property(t => t.Property).Name("name-03")
        );

        Assert.Contains($"\"{EncodeName("name-03")}\":\"{PROP_VALUE}\"", json);
    }

    [Fact]
    public void LastNameChangeWinsWithMultipleAppends()
    {
        var options = new JsonSerializerOptions();
        options.Configure<SampleClass>(b => b.Property(t => t.Property).Name("name-01"))
            .Configure<SampleClass>(b => b.Property(t => t.Property).Name("name-02"))
            .Configure<SampleClass>(b => b.Property(t => t.Property).Name("name-03"));

        var json = JsonSerializer.Serialize(new SampleClass(), options);
        Assert.NotNull(json);
        Assert.NotEmpty(json);

        Assert.Contains($"\"name-03\":\"{PROP_VALUE}\"", json);
    }

    [Fact]
    public void DeserializesWithNewName()
    {
        const string NAME = "name-01";
        const string VALUE = "new-value";

        var options = new JsonSerializerOptions();
        options.Configure<SampleClass>(b => b.Property(t => t.Property).Name(NAME));

        var json = JsonSerializer.Serialize(new SampleClass { Property = VALUE }, options);
        Assert.Contains($"\"{NAME}\":\"{VALUE}\"", json);

        // Assert w/ options
        var actual = JsonSerializer.Deserialize<SampleClass>(json, options);
        Assert.NotNull(actual);
        Assert.Equal(VALUE, actual.Property);

        // Assert w/o options
        actual = JsonSerializer.Deserialize<SampleClass>(json);
        Assert.NotNull(actual);
        Assert.Equal(PROP_VALUE, actual.Property);      // Defaults to PROP_VALUE when not found
    }

    private string Serialize(Action<JsonEntityBuilder<SampleClass>>? configure = null)
    {
        var options = new JsonSerializerOptions();
        options.IncludeFields = true;
        options.Configure<SampleClass>(b => configure?.Invoke(b));

        var json = JsonSerializer.Serialize(new SampleClass(), options);
        Assert.NotNull(json);
        Assert.NotEmpty(json);

        return json;
    }

    private string EncodeName(string name)
    {
        var encodedName = JsonSerializer.Serialize(name);
        return encodedName[1..^1];  // Remove Quotes
    }

    public record SampleClass
    {
        public string Property { get; set; } = PROP_VALUE;
        public string Field = PROP_VALUE;
    }
}

