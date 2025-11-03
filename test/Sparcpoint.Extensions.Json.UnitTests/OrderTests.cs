using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sparcpoint.Extensions.Json.UnitTests;

public class OrderTests
{
    private const string PROP_VALUE = "PropertyValue";

    [Fact]
    public void ConfirmNaturalOrder()
    {
        var json = Serialize();
        AssertOrder(json, nameof(SampleClass.Property01), nameof(SampleClass.Property02), nameof(SampleClass.Property03));
    }

    [Fact]
    public void CanMoveAttributeOrderedProperty()
    {
        var json = Serialize(b => b.Property(t => t.Property03).Order(-10));
        AssertOrder(json, nameof(SampleClass.Property03), nameof(SampleClass.Property01), nameof(SampleClass.Property02));
    }

    [Fact]
    public void LowerOrderForFirstPropertyDoesNotMove()
    {
        var json = Serialize(b => b.Property(t => t.Property01).Order(-10));
        AssertOrder(json, nameof(SampleClass.Property01), nameof(SampleClass.Property02), nameof(SampleClass.Property03));
    }

    [Fact]
    public void CanMoveMiddleToFront()
    {
        var json = Serialize(b => b.Property(t => t.Property02).Order(-10));
        AssertOrder(json, nameof(SampleClass.Property02), nameof(SampleClass.Property01), nameof(SampleClass.Property03));
    }

    [Fact]
    public void CanChangeAllOrders()
    {
        var json = Serialize(b => b
            .Property(t => t.Property01).Order(100)
            .Property(t => t.Property02).Order(-100)
            .Property(t => t.Property03).Order(5)
        );
        AssertOrder(json, nameof(SampleClass.Property02), nameof(SampleClass.Property03), nameof(SampleClass.Property01));
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

    private void AssertOrder(string actual, params string[] propNames)
    {
        List<string> props = new();
        foreach(var name in propNames)
        {
            props.Add($"\"{name}\":\"{PROP_VALUE}\"");
        }

        var expected = "{" + string.Join(",", props) + "}";
        Assert.Equal(expected, actual);
    }

    public class SampleClass
    {
        public string Property01 { get; set; } = PROP_VALUE;

        [JsonPropertyOrder(3)]
        public string Property03 { get; set; } = PROP_VALUE;

        public string Property02 { get; set; } = PROP_VALUE;
    }
}

