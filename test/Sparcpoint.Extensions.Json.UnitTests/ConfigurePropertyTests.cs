using System.Text.Json;

namespace Sparcpoint.Extensions.Json.UnitTests;

public class ConfigurePropertyTests
{
    [Fact]
    public void CanProvidePropertyExpression()
    {
        var options = new JsonSerializerOptions();
        options.Configure<SampleClass>(b => b
            .Property(t => t.Property)
        );
    }

    [Fact]
    public void CanProvideFieldExpression()
    {
        var options = new JsonSerializerOptions();
        options.Configure<SampleClass>(b => b
            .Property(t => t.Field)
        );
    }

    [Fact]
    public void ThrowsOnFunctionExpression()
    {
        var options = new JsonSerializerOptions();
        Assert.Throws<InvalidOperationException>(() =>
        {
            options.Configure<SampleClass>(b => b
                .Property(t => t.Function())
            );
        });
    }

    [Fact]
    public void ThrowsOnConstantExpression()
    {
        var options = new JsonSerializerOptions();
        Assert.Throws<InvalidOperationException>(() =>
        {
            options.Configure<SampleClass>(b => b
                .Property(t => SampleClass.CONSTANT)
            );
        });
    }

    [Fact]
    public void ThrowsOnConstructorExpression()
    {
        var options = new JsonSerializerOptions();
        Assert.Throws<InvalidOperationException>(() =>
        {
            options.Configure<SampleClass>(b => b
                .Property(t => new SampleClass())
            );
        });
    }

    public record SampleClass
    {
        public const string CONSTANT = "Constant";

        public string Property { get; set; } = string.Empty;
        public string Field = string.Empty;
        public string Function() { return string.Empty; }
    }
}
